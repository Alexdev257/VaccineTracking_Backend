using System.Text;
using System.Web;
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

        [HttpPost("add-booking")]
        public async Task<IActionResult> AddBooking([FromBody] AddBooking addBooking)
        {
            OrderInfoModel orderInfo = (await _bookingService.AddBooking(addBooking))!;
            // Chose payment method
            if (addBooking.paymentId > 1 && addBooking.paymentId < 5)
            {
                var client = _httpClientFactory.CreateClient();

                System.Console.WriteLine((PaymentEnum)addBooking.paymentId);
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
            else return BadRequest();

            // If payment method is by cash
        }

        [HttpPost("add-booking-by-staff")]
        public async Task<string> AddBookingStaff([FromBody] AddBooking addBooking)
        {
            OrderInfoModel orderInfo = (await _bookingService.AddBooking(addBooking))!;
            string bookingID = orderInfo.BookingID;
            decimal amount = orderInfo.Amount;
            string paymentID = TimeProvider.GetVietnamNow().Ticks.ToString();
            string trancasionID = paymentID;
            string userID = orderInfo.GuestName.Split(" ")[0];
            int paymentMethod = (int)PaymentEnum.Cash;
            string currency = "VND";
            DateTime paymentDate = TimeProvider.GetVietnamNow();

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
                Status = PaymentStatusEnum.Success.ToString()
            };

            await _paymentRepository.AddPayment(payment);
            await _bookingService.UpdateBookingStatus(bookingID, BookingEnum.Success.ToString());
            return BookingEnum.Success.ToString();
        }


        [HttpGet("booking-history/{userID}")]
        public async Task<IActionResult> GetAllBookingByUser([FromRoute] int userID)
        {
            var bookingList = await _bookingService.GetBookingByUserAsync(userID);
            if (bookingList.IsNullOrEmpty()) return BadRequest("Dont have");
            return Ok(bookingList);
        }
        [HttpGet("booking-history-staff/{userID}")]
        public async Task<IActionResult> GetAllBookingByUserStaff([FromRoute] int userID)
        {
            var bookingList = await _bookingService.GetBookingByUserAsyncStaff(userID);
            if (bookingList.IsNullOrEmpty()) return BadRequest("Dont have");
            return Ok(bookingList);
        }
        [HttpGet("get-all-booking")]
        public async Task<IActionResult> GetAllForStaff()
        {
            var bookingList = await _bookingService.GetAllBookingForStaff();
            return Ok(bookingList);
        }
        // [HttpPatch("mark-as-complete")]
    }
}