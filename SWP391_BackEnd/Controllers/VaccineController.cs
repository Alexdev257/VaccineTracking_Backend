using ClassLib.DTO.Vaccine;
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

        [HttpGet("getAllVaccines")]
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

        [HttpGet("getVaccineById/{id}")]
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

        [HttpPost("createVaccine")]
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

        [HttpPut("updateVaccineById/{id}")]
        public async Task<IActionResult> UpdateVaccine([FromBody] UpdateVaccine rq, int id)
        {
            try
            {
                var vaccine = await _vaccineService.UpdateVaccine(rq, id);
                return Ok(vaccine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("deleteVaccineById/{id}")]
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

        [HttpGet("by-age/{age}")]
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
