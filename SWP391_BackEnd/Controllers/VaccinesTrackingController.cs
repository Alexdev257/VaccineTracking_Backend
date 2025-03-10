using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.VaccineTracking;
using ClassLib.Helpers;
using ClassLib.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace SWP391_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VaccinesTrackingController : ControllerBase
    {
        private readonly VaccinesTrackingService _vaccinesTrackingService;
        public VaccinesTrackingController(VaccinesTrackingService vaccinesTrackingService)
        {
            _vaccinesTrackingService = vaccinesTrackingService;
        }

        // admin
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _vaccinesTrackingService.GetVaccinesTrackingAsync();
            if (result.IsNullOrEmpty()) return BadRequest();
            return Ok(result);
        }

        //user
        [HttpGet("get-by-parent-id/{id}")]
        public async Task<IActionResult> GetByParentId(int id)
        {
            var result = await _vaccinesTrackingService.GetVaccinesTrackingByParentIdAsync(id);
            if (result.IsNullOrEmpty()) return BadRequest();
            return Ok(result);
        }


        // 
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _vaccinesTrackingService.GetVaccinesTrackingByIdAsync(id);
            if (result == null) return BadRequest();
            return Ok(result);
        }

        // admin reaction or success + cancel
        [HttpPut("update-vaccine-admin/{id}")]
        public async Task<IActionResult> UpdateVaccinesTracking([FromRoute] int id, [FromBody] UpdateVaccineTracking updateVaccineTracking)
        {
            var respone = await _vaccinesTrackingService.UpdateVaccinesTrackingAsync(id, updateVaccineTracking);
            if (respone == false) return BadRequest();
            return Ok(respone);
        }


        // user reaction or cancel
        [HttpPut("update-vaccine-user/{id}")]
        public async Task<IActionResult> UpdateReactionForUser([FromRoute] int id, [FromBody] UpdateVaccineTrackingUser updateVaccineTrackingUser)
        {
            UpdateVaccineTracking updateVaccineTracking = ConvertHelpers.ConvertToUpdateVaccineTracking(updateVaccineTrackingUser);
            var respone = await _vaccinesTrackingService.UpdateVaccinesTrackingAsync(id, ConvertHelpers.ConvertToUpdateVaccineTracking(updateVaccineTrackingUser));
            if (respone == false) return BadRequest();
            else return Ok("Update Success");
        }

    }
}