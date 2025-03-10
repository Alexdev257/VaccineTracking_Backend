using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.DTO.Booking;
using ClassLib.DTO.Payment;
using ClassLib.Enum;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories;
using ClassLib.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TimeProvider = ClassLib.Helpers.TimeProvider;

namespace SWP391_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly PaymentRepository _paymentRepository;
        public BookingController(BookingService bookingService, IHttpClientFactory httpClientFactory, PaymentRepository paymentRepository)
        {
            _bookingService = bookingService;
            _httpClientFactory = httpClientFactory;
            _paymentRepository = paymentRepository;
        }


        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] BookingQuerryObject collection)
        {
            return Ok(await _bookingService.GetByQuerry(collection));
        }


        [HttpPost("add-booking")]
        public async Task<IActionResult> AddBooking([FromBody] AddBooking addBooking)
        {
            OrderInfoModel orderInfo;

            // Create Model Info Include Create Booking Data, Booking Child Data, Booking Vaccine Data, ...
            if (addBooking.BookingID == 0) orderInfo = (await _bookingService.AddBooking(addBooking))!;

            // Pay again for Booking have "Pending" status
            else orderInfo = (await _bookingService.RepurchaseBooking(addBooking.BookingID))!;


            // Chose payment method
            if (addBooking.paymentId != 0)
            {
                var client = _httpClientFactory.CreateClient();

                var paymentApi = $"http://localhost:5272/api/Payment/create/{((PaymentEnum)addBooking.paymentId).ToString()}";

                var jsonRequest = JsonConvert.SerializeObject(orderInfo);

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(paymentApi, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseData))
                    {
                        return Ok(responseData);
                    }
                }

                return BadRequest(new { message = "Failed to initiate payment" });
            }

            // If payment method is by cash
            else
            {
                string bookingID = orderInfo.BookingID;
                decimal amount = orderInfo.Amount;
                string paymentID = TimeProvider.GetVietnamNow().Ticks.ToString();
                string trancasionID = paymentID;
                string userID = orderInfo.GuestName.Split(" ")[0];
                int paymentMethod = (int)PaymentEnum.Cash;
                string currency = "VND";
                DateTime paymentDate = TimeProvider.GetVietnamNow();
                string status = "Pending";

                Payment payment = new Payment()
                {
                    PaymentId = paymentID,
                    BookingId = int.Parse(bookingID),
                    TransactionId = trancasionID,
                    TotalPrice = amount,
                    PayerId = userID,
                    PaymentMethod = paymentMethod,
                    Currency = currency,
                    PaymentDate = paymentDate,
                    Status = status
                };

                await _paymentRepository.AddPayment(payment);
            }

            return Ok(orderInfo);

        }

        [HttpGet("booking-history/{userID}")]
        public async Task<IActionResult> GetAllBookingByUser([FromRoute] int userID)
        {
            var bookingList = await _bookingService.GetBookingByUserAsync(userID);
            if (bookingList.IsNullOrEmpty()) return BadRequest("Dont have");
            return Ok(bookingList);
        }
    }
}