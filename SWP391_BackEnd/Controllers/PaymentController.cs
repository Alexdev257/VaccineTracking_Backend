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
using Microsoft.OpenApi.Extensions;
using PayPal.v1.Orders;

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
        [HttpPost("refund")]
        public async Task<IActionResult> RefundPayment([FromBody] RefundModelRequest refundModelRequest)
        {

            string paymentName = await _paymentRepository.GetPaymentNameOfBooking(refundModelRequest.BookingID);

            if (!_payment.TryGetValue(paymentName.ToLower(), out var paymentService))
            {
                return BadRequest("Invalid payment method.");
            }

            var payment = await _paymentRepository.GetByBookingIDAsync(int.Parse(refundModelRequest.BookingID));

            if (payment!.Status.Contains("refund", StringComparison.OrdinalIgnoreCase)) return BadRequest("The Booking is already refund");
            var refundModel = ConvertHelpers.convertToRefundModel(payment!, (double)((refundModelRequest.paymentStatusEnum == 1) ? payment.TotalPrice * 1m : payment.TotalPrice * 0.5m));

            var refundDetail = await paymentService.CreateRefund(refundModel, HttpContext);

            System.Console.WriteLine("Payment ID:" + payment.PaymentId);

            if (!refundDetail.IsNullOrEmpty()) System.Console.WriteLine((await _paymentRepository.UpdateStatusPayment(payment.PaymentId, "Refund")) ? "Hihi" : "Haha");

            return Ok(refundDetail);
        }
    }
}