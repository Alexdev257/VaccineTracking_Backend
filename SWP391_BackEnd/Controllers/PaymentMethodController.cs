using ClassLib.DTO.PaymentMethod;
using ClassLib.Service;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentMethodController : ControllerBase
    {
        private readonly PaymentMethodService _paymentMethodService;

        public PaymentMethodController(PaymentMethodService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
        }

        // Get all payment methods
        [HttpGet]
        public async Task<IActionResult> GetPaymentMethod()
        {
            return Ok(await _paymentMethodService.getAll());
        }

        // Get payment method by ID
        [HttpGet("GetByID/{id}")]
        public async Task<IActionResult> GetPaymentMethodById([FromRoute] int id)
        {
            return Ok(await _paymentMethodService.getPaymentMethodById(id));
        }


        // Get payment method by name
        [HttpGet("GetByName/{name}")]
        public async Task<IActionResult> GetPaymentMethodByName([FromRoute] string name)
        {
            return Ok(await _paymentMethodService.getPaymentMethodByName(name));
        }

        // Create a new payment method
        [HttpPost]
        public async Task<IActionResult> CreatePaymentMethod([FromBody] AddPaymentMethod addPaymentMethod)
        {
            return Ok(await _paymentMethodService.addPaymentMethod(addPaymentMethod));
        }

        // Update a payment method
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePaymentMethod([FromRoute] int id, [FromBody] UpdatePaymentMethod updatePaymentMethod)
        {
            return Ok(await _paymentMethodService.updatePaymentMethod(id, updatePaymentMethod));
        }
    }
}