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
            else if ((int)PaymentEnum.Cash == addBooking.paymentId)
            {
                string bookingID = orderInfo.BookingID;
                decimal amount = orderInfo.Amount;
                string paymentID = TimeProvider.GetVietnamNow().Ticks.ToString();
                string trancasionID = paymentID;
                string status = "Pending";

                RespondModel response = new RespondModel()
                {
                    BookingID = bookingID,
                    Amount = amount.ToString(),
                    TrancasionID = trancasionID,
                    Message = status,
                    OrderId = paymentID,
                    OrderDescription = "",
                };

                UriBuilder uriBuilder = new UriBuilder($"http://localhost:5173/confirm/pending");

                var queryParams = HttpUtility.ParseQueryString(string.Empty);

                foreach (var prop in response.GetType().GetProperties())
                {
                    var value = prop.GetValue(response)?.ToString();
                    if (value != null)
                    {
                        queryParams[prop.Name] = value;
                    }
                }

                uriBuilder.Query = queryParams.ToString();

                // return Redirect(uriBuilder.ToString());

                return Ok(uriBuilder.ToString());
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

        [HttpPatch("update-booking-details")]
        public async Task<IActionResult> UpdateBookingDetails(UpdateBooking updateBooking)
        {
            return Ok(await _bookingService.UpdateBookingDetails(updateBooking));
        }

        [HttpDelete("soft-delete-booking/{bookingID}")]
        public async Task<IActionResult> DeleteBooking([FromRoute] string bookingID)
        {
            var result =await _bookingService.GetBookingById(int.Parse(bookingID));
            if (result == null) return BadRequest("Booking not found");
            if (result.Status.ToString() == BookingEnum.Success.ToString()) return BadRequest("Booking is already success");
            return Ok(await _bookingService.SoftDeleteBooking(bookingID));
        }
    
        [HttpDelete("hard-booking/{bookingID}")]
        public async Task<IActionResult> HardDeleteBooking([FromRoute] string bookingID)
        {
            var result = await _bookingService.GetBookingById(int.Parse(bookingID));
            if (result == null) return BadRequest("Booking not found");
            if ( !result.IsDeleted) return BadRequest("Booking is can not hard delete");
            return Ok(await _bookingService.HardDeleteBooking(bookingID));
        }
    
        [HttpGet("get-all-booking-admin")]
        public async Task<IActionResult> GetAllForAdmin()
        {
            var bookingList = await _bookingService.GetAllBookingForAdmin();
            return Ok(bookingList);
        }
    }
}