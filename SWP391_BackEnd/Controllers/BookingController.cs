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
using ClassLib.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace SWP391_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
        private readonly IHttpClientFactory _httpClientFactory;
        public BookingController(BookingService bookingService, IHttpClientFactory httpClientFactory)
        {
            _bookingService = bookingService;
            _httpClientFactory = httpClientFactory;
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
            if (addBooking.BookingID == 0) orderInfo = (await _bookingService.AddBooking(addBooking))!;
            else orderInfo = (await _bookingService.RepurchaseBooking(addBooking.BookingID))!;

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

            return Ok("Payment By Cash success");

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