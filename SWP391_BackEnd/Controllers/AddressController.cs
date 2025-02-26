using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLib.DTO.Address;
using ClassLib.Models;
using ClassLib.Service.Addresses;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/addresses")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly AddressService _addressService;

        public AddressController(AddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("getAllAddress")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAllAddresses()
        {
            return await _addressService.GetAllAddresses();
        }

        [HttpGet("getAddressById/{id}")]
        public async Task<ActionResult<Address>> GetAddressById(int id)
        {
            var address = await _addressService.GetAddressById(id);
            if (address == null) return NotFound();
            return address;
        }

        [HttpPost("addAddresses")]
        public async Task<ActionResult<Address>> AddAddress([FromBody] AddAddress addAddress)
        {
            if (addAddress == null) return BadRequest();

            var createdAddress = await _addressService.AddAddress(addAddress);
            return CreatedAtAction(nameof(GetAddressById), new { id = createdAddress.Id }, createdAddress);
        }

        [HttpPut("updateById{id}")]
        public async Task<ActionResult<Address>> UpdateAddress(int id, [FromBody] AddAddress updateAddress)
        {
            if (updateAddress == null) return BadRequest();

            var updatedAddress = await _addressService.UpdateAddress(id, updateAddress);
            if (updatedAddress == null) return NotFound();
            return updatedAddress;
        }

        [HttpDelete("deleteById{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var result = await _addressService.DeleteAddress(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
