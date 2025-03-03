using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.PaymentMethod;
using ClassLib.Models;
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
        public ActionResult GetPaymentMethod()
        {
            return Ok(_paymentMethodService.getAll());
        }

        // Get payment method by ID
        [HttpGet("GetByID/{id}")]
        public ActionResult GetPaymentMethodById([FromRoute] int id)
        {
            return Ok(_paymentMethodService.getPaymentMethodById(id));
        }


        // Get payment method by name
        [HttpGet("GetByName/{name}")]
        public ActionResult GetPaymentMethodByName([FromRoute] string name)
        {
            return Ok(_paymentMethodService.getPaymentMethodByName(name));
        }

        // Create a new payment method
        [HttpPost]
        public ActionResult CreatePaymentMethod([FromBody] AddPaymentMethod addPaymentMethod)
        {
            return Ok(_paymentMethodService.addPaymentMethod(addPaymentMethod));
        }

        // Update a payment method
        [HttpPut("{id}")]
        public ActionResult UpdatePaymentMethod([FromRoute] int id, [FromBody] UpdatePaymentMethod updatePaymentMethod)
        {
            return Ok(_paymentMethodService.updatePaymentMethod(id, updatePaymentMethod));
        }
    }
}