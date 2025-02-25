using ClassLib.DTO.Vaccine;
using ClassLib.Service.Vaccines;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("getAllVacines")]
        public async Task<IActionResult> GetVaccines()
        {
            var vaccines = await _vaccineService.GetAllVaccines();
            if (vaccines == null || vaccines.Count == 0)
            {
                return NotFound("No vaccines found.");
            }
            return Ok(vaccines);
        }

        [HttpGet("getVacineById{id}")]
        public async Task<IActionResult> GetVaccines(int id)
        {
            var vaccine = await _vaccineService.GetVaccineById(id);
            if (vaccine == null)
            {
                return NotFound("No vaccine found.");
            }
            return Ok(vaccine);
        }
        [HttpPost("createVaccine")]
        public async Task<IActionResult> CreateVaccine([FromBody] CreateVaccine rq)
        {
            var vaccine = await _vaccineService.CreateVaccine(rq);
            return Ok(vaccine);
        }
        [HttpPut("updateVaccineById{id}")]
        public async Task<IActionResult> UpdateVaccine([FromBody] UpdateVaccine rq, int id)
        {
            var vaccine = await _vaccineService.UpdateVaccine(rq, id);
            return Ok(vaccine);
        }

        [HttpDelete("deleteVaccineById{id}")]
        public async Task<IActionResult> DeleteVaccine(int id)
        {
            var result = await _vaccineService.DeleteVaccine(id);
            return Ok(result);
        }
        [HttpGet("by-age/{age}")]
        public async Task<IActionResult> GetVaccinesByAge(int age)
        {
            var vaccines = await _vaccineService.GetVaccinesByAge(age);
            if (vaccines == null || vaccines.Count == 0)
            {
                return NotFound("No vaccines found for this age.");
            }
            return Ok(vaccines);
        }
    }
}
