using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.Payment;
using ClassLib.Models;
using ClassLib.Service;
using ClassLib.Service.PaymentService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SWP391_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IDictionary<string, IPaymentServices> _payment;
        private readonly BookingService _bookingService;

        public PaymentController(IEnumerable<IPaymentServices> payment, BookingService bookingService)
        {
            _bookingService = bookingService;
            _payment = payment.ToDictionary(s => s.PaymentName().ToLower());
        }

        [HttpPost("create/{payment_name}")]
        public async Task<IActionResult> CreatePaymentURL([FromRoute] string payment_name, [FromBody] OrderInfoModel orderInfoModel)
        {
            if (!_payment.TryGetValue(payment_name.ToLower(), out var paymentService))
            {
                return BadRequest("Invalid payment method.");
            }

            var paymentUrl = await paymentService.CreatePaymentURL(orderInfoModel, HttpContext);
            return Ok(paymentUrl);
        }
        [HttpGet("callback/{payment_name}")]
        public async Task<IActionResult> Callback([FromRoute] string payment_name)
        {
            if (!_payment.TryGetValue(payment_name.ToLower(), out var paymentService))
            {
                return BadRequest("Invalid payment method.");
            }

            var response = await paymentService.GetPaymentStatus(Request.Query);
            if (response == null)
            {
                return BadRequest("Invalid payment method.");
            }
            var booking = await _bookingService.UpdateBookingStatus(response.BookingID, response.Message);
            if (booking == null)
            {
                return BadRequest("Invalid booking id.");
            }
            return Ok(response);
        }


    }
}