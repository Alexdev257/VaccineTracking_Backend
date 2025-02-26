using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentAPI.Model;
using PaymentAPI.Services;

namespace PaymentAPI.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentControllers : ControllerBase
    {
        private readonly IDictionary<string, IPaymentServices> _payment;

        public PaymentControllers(IEnumerable<IPaymentServices> payment)
        {
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
            return Ok(new { PayURL = paymentUrl });
        }
        [HttpGet("callback/{payment_name}")]
        public async Task<IActionResult> Callback([FromRoute] string payment_name)
        {
            if (!_payment.TryGetValue(payment_name.ToLower(), out var paymentService))
            {
                return BadRequest("Invalid payment method.");
            }

            var response = await paymentService.GetPaymentStatus(Request.Query);
            return Ok(response);
        }


    }
}