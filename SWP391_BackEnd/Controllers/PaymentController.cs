using System.Web;
using ClassLib.DTO.Payment;
using ClassLib.Enum;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories;
using ClassLib.Service;
using ClassLib.Service.PaymentService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TimeProvider = ClassLib.Helpers.TimeProvider;

namespace SWP391_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IDictionary<string, IPaymentServices> _payment;
        private readonly BookingService _bookingService;
        private readonly PaymentRepository _paymentRepository;
        private readonly VaccinesTrackingService _vaccinesTrackingService;

        private readonly PaymentMethodService _paymentMethodServices;
        public PaymentController(IEnumerable<IPaymentServices> payment
                                , BookingService bookingService
                                , PaymentRepository paymentRepository
                                , VaccinesTrackingService vaccinesTrackingService
                                , PaymentMethodService paymentMethodService)
        {
            _bookingService = bookingService;
            _payment = payment.ToDictionary(s => s.PaymentName().ToLower());
            _paymentRepository = paymentRepository;
            _vaccinesTrackingService = vaccinesTrackingService;
            _paymentMethodServices = paymentMethodService;
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

            await _bookingService.UpdateBookingStatus(response.BookingID, response.Message);

            UriBuilder uriBuilder = new UriBuilder($"http://localhost:5173/confirm/{(response.Message.ToLower() == "success" ? "success" : "failed")}");

            var queryParams = HttpUtility.ParseQueryString(string.Empty);

            // Use reflection to get all properties from the response object
            foreach (var prop in response.GetType().GetProperties())
            {
                var value = prop.GetValue(response)?.ToString();
                if (value != null)
                {
                    queryParams[prop.Name] = value;
                }
            }

            uriBuilder.Query = queryParams.ToString();

            // Redirect to frontend with all response data in URL
            return Redirect(uriBuilder.ToString());

            // return Ok(response);
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

            var refundModel = ConvertHelpers.convertToRefundModel(payment!, (double)((refundModelRequest.paymentStatusEnum == (int)PaymentStatusEnum.FullyRefunded) ? payment.TotalPrice * 1m : payment.TotalPrice * 0.5m), refundModelRequest.paymentStatusEnum);

            var refundDetail = await paymentService.CreateRefund(refundModel, HttpContext);

            if (!refundDetail.IsNullOrEmpty() && refundDetail.ToString().ToLower() == "success")
            {
                var result = await _paymentRepository.UpdateStatusPayment(payment.PaymentId, PaymentStatusEnum.Refunded.ToString());
                await _bookingService.UpdateBookingStatus(result.BookingId.ToString(), BookingEnum.Refund.ToString());
                await _vaccinesTrackingService.VaccinesTrackingRefund(result.BookingId, VaccinesTrackingEnum.Cancel);
            }

            return Ok(refundDetail);
        }


        [HttpPost("refund-by-staff")]
        public async Task<IActionResult> RefundVnPay([FromBody] RefundModelRequest refundModelRequest)
        {
            var payment = await _paymentRepository.GetByBookingIDAsync(int.Parse(refundModelRequest.BookingID));

            if (payment!.Status.Contains("refund", StringComparison.OrdinalIgnoreCase)) return BadRequest("The Booking is already refund");

            var refundModel = ConvertHelpers.convertToRefundModel(payment!, (double)((refundModelRequest.paymentStatusEnum == (int)PaymentStatusEnum.FullyRefunded) ? payment.TotalPrice * 1m : payment.TotalPrice * 0.5m), refundModelRequest.paymentStatusEnum);

            Payment paymentRefundModel = new Payment()
            {
                PayerId = refundModel.payerID,
                TotalPrice = (decimal)refundModel.amount * -1,
                PaymentId = TimeProvider.GetVietnamNow().Ticks.ToString(),
                PaymentDate = TimeProvider.GetVietnamNow(),
                TransactionId = TimeProvider.GetVietnamNow().Ticks.ToString(),
                Currency = refundModel.currency,
                PaymentMethod = (await _paymentMethodServices.getPaymentMethodByName(PaymentEnum.Cash.ToString()))!.Id,
                Status = ((PaymentStatusEnum)refundModel.RefundType).ToString(),
                BookingId = (await _paymentRepository.GetByIDAsync(refundModel.paymentID))!.BookingId,
            };
            await _paymentRepository.AddPayment(paymentRefundModel);

            var result = await _paymentRepository.UpdateStatusPayment(payment.PaymentId, PaymentStatusEnum.Refunded.ToString());
            await _bookingService.UpdateBookingStatus(result.BookingId.ToString(), BookingEnum.Refund.ToString());
            await _vaccinesTrackingService.VaccinesTrackingRefund(result.BookingId, VaccinesTrackingEnum.Cancel);

            return Ok("Success");
        }
    }
}