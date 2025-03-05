﻿using ClassLib.DTO.Vaccine;
using ClassLib.Service.Vaccines;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccineController : ControllerBase
    {
        private readonly VaccineService _vaccineService;

        public VaccineController(VaccineService vaccineService)
        {
            _vaccineService = vaccineService ?? throw new ArgumentNullException(nameof(vaccineService));
        }

        [HttpGet("get-all-vaccines")]
        public async Task<IActionResult> GetVaccines()
        {
            try
            {
                var vaccines = await _vaccineService.GetAllVaccines();
                if (vaccines == null || vaccines.Count == 0)
                {
                    return NotFound("No vaccines found.");
                }
                return Ok(vaccines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("get-vaccine-by-id/{id}")]
        public async Task<IActionResult> GetVaccine(int id)
        {
            try
            {
                var vaccine = await _vaccineService.GetVaccineById(id);
                if (vaccine == null)
                {
                    return NotFound("No vaccine found.");
                }
                return Ok(vaccine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("create-vaccine")]
        public async Task<IActionResult> CreateVaccine([FromBody] CreateVaccine rq)
        {
            try
            {
                var vaccine = await _vaccineService.CreateVaccine(rq);
                return Ok(vaccine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        //[HttpPut("updateVaccineById/{id}")]
        //public async Task<IActionResult> UpdateVaccine([FromBody] UpdateVaccine rq, int id)
        //{
        //    try
        //    {
        //        var vaccine = await _vaccineService.UpdateVaccine(rq, id);
        //        return Ok(vaccine);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal Server Error: {ex.Message}");
        //    }
        //}

        [HttpPut("update-vaccine/{id}")]
        public async Task<IActionResult> UpdateVaccineController(int id, [FromBody] UpdateVaccine request)
        {
            try
            {
                var rs = await _vaccineService.UdateVaccineAsync(id, request);
                return Ok("Update Successfully");
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete-vaccine-by-id/{id}")]
        public async Task<IActionResult> DeleteVaccine(int id)
        {
            try
            {
                var result = await _vaccineService.DeleteVaccine(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("get-vaccine-by-age/{age}")]
        public async Task<IActionResult> GetVaccinesByAge(int age)
        {
            try
            {
                var vaccines = await _vaccineService.GetVaccinesByAge(age);
                if (vaccines == null || vaccines.Count == 0)
                {
                    return NotFound("No vaccines found for this age.");
                }
                return Ok(vaccines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
