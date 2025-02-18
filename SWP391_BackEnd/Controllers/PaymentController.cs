using ClassLib.DTO.Payment;
using ClassLib.Service.Momo;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMomoService _momoService;

        public PaymentController(IMomoService momoService)
        {
            _momoService = momoService;
        }

        [HttpPost("{paymentName}/create")]
        public async Task<IActionResult> CreatePaymentUrl([FromRoute] string paymentName, [FromBody] OrderInfoModel model)
        {
            if (paymentName == "momo")
            {
                var response = await _momoService.CreatePaymentAsync(model);
                return Ok(new { PayUrl = response.PayUrl });
            }
            return BadRequest("Invalid payment method");
        }

        [HttpGet("callback")]
        public IActionResult PaymentCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(Request.Query);
            return Ok(response);  // Return JSON response
        }
    }
}
