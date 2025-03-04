using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.Payment;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories;
using ClassLib.Service;
using ClassLib.Service.PaymentService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace SWP391_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IDictionary<string, IPaymentServices> _payment;
        private readonly BookingService _bookingService;

        private readonly PaymentRepository _paymentRepository;

        public PaymentController(IEnumerable<IPaymentServices> payment
                                , BookingService bookingService
                                , PaymentRepository paymentRepository)
        {
            _bookingService = bookingService;
            _payment = payment.ToDictionary(s => s.PaymentName().ToLower());
            _paymentRepository = paymentRepository;
        }

        // Create payment URL
        [HttpPost("create/{payment_name}")]
        public async Task<IActionResult> CreatePaymentURL([FromRoute] string payment_name, [FromBody] OrderInfoModel orderInfoModel)
        {
            if (!_payment.TryGetValue(payment_name.ToLower(), out var paymentService))
            {
                return BadRequest("Invalid payment method.");
            }

            var paymentUrl = await paymentService.CreatePaymentURL(orderInfoModel, HttpContext);

            // if( paymentUrl == null ) => direct to fail
            return Ok(paymentUrl);
        }

        // Get payment status + Update booking status
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


        // Refund Money
        [HttpPost("refund/{payment_name}/{bookingID}")]
        public async Task<IActionResult> RefundPayment([FromRoute] string payment_name, string bookingID, [FromBody] double amount)
        {
            if (!_payment.TryGetValue(payment_name.ToLower(), out var paymentService))
            {
                return BadRequest("Invalid payment method.");
            }
            var payment = await _paymentRepository.GetByBookingIDAsync(int.Parse(bookingID));

            if (payment!.Status == "Refund") return BadRequest("The Booking is already refund");

            var refundModel = ConvertHelpers.convertToRefundModel(payment!, amount);

            var refundDetail = await paymentService.CreateRefund(refundModel, HttpContext);

            if (!refundDetail.IsNullOrEmpty()) await _paymentRepository.UpdateStatusPayment(payment.PaymentId, "Refund");

            return Ok(refundDetail);
        }
    }
}