using ClassLib.DTO.Payment;
using ClassLib.Service.Momo;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    public class PaymentController : ControllerBase
    {
        private readonly IMomoService _momoService;

        public PaymentController(IMomoService momoService)
        {
            _momoService = momoService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] OrderInfoModel model)
        {
            var response = await _momoService.CreatePaymentAsync(model);
            return Ok(new { PayUrl = response.PayUrl });
        }

        [HttpGet("callback")]
        public IActionResult PaymentCallBack([FromQuery] OrderInfoModel model)
        {
            var response = _momoService.PaymentExecuteAsync(model);
            return Ok(response);  // Return JSON response
        }
    }
}
