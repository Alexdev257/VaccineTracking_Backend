using ClassLib.DTO.Address;
using ClassLib.Models;
using ClassLib.Service.Addresses;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly AddressService _addressService;

        public AddressController(AddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("get-all-address")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAllAddresses()
        {
            return await _addressService.GetAllAddresses();
        }

        [HttpGet("get-address-by-id/{id}")]
        public async Task<ActionResult<Address>> GetAddressById(int id)
        {
            var address = await _addressService.GetAddressById(id);
            if (address == null) return NotFound();
            return address;
        }

        [HttpPost("create-address")]
        public async Task<ActionResult<Address>> AddAddress([FromBody] AddAddress addAddress)
        {
            if (addAddress == null) return BadRequest();

            var createdAddress = await _addressService.AddAddress(addAddress);
            return CreatedAtAction(nameof(GetAddressById), new { id = createdAddress.Id }, createdAddress);
        }

        [HttpPut("update-address-by-id{id}")]
        public async Task<ActionResult<Address>> UpdateAddress(int id, [FromBody] AddAddress updateAddress)
        {
            if (updateAddress == null) return BadRequest();

            var updatedAddress = await _addressService.UpdateAddress(id, updateAddress);
            if (updatedAddress == null) return NotFound();
            return updatedAddress;
        }

        [HttpDelete("delete-address-by-id{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var result = await _addressService.DeleteAddress(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("/Map")]
        public async Task<IActionResult> GetAllMap()
        {
            try
            {
                var map = await _addressService.GetAllMap();
                return Ok(map);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
