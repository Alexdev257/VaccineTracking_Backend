using ClassLib.DTO.Vaccine;
using ClassLib.DTO.VaccineCombo;
using ClassLib.Service.VaccineCombo;
using ClassLib.Service.Vaccines;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccineComboController : ControllerBase
    {
        private readonly VaccineComboService _vaccineComboService;

        public VaccineComboController(VaccineComboService vaccineComboService)
        {
            _vaccineComboService = vaccineComboService ?? throw new ArgumentNullException(nameof(vaccineComboService));
        }

        [HttpGet("getVaccineCombo")]
        public async Task<IActionResult> GetVaccineCombos()
        {
            var combos = await _vaccineComboService.GetAllVaccineCombo();
            if (combos == null || combos.Count == 0)
            {
                return NotFound("No combos found.");
            }
            return Ok(combos);
        }

        [HttpGet("getVaccineById{id}")]
        public async Task<IActionResult> GetVaccineCombo(int id)
        {
            var combo = await _vaccineComboService.GetVaccineComboById(id);
            if (combo == null)
            {
                return NotFound("No combo found.");
            }
            return Ok(combo);
        }

        [HttpPost("createVaccine")]
        public async Task<IActionResult> CreateVaccine([FromBody] CreateVaccineCombo rq)
        {
            var combo = await _vaccineComboService.CreateVaccineCombo(rq);
            return Ok(combo);
        }

        [HttpPut("updateVaccineById{id}")]
        public async Task<IActionResult> UpdateVaccine([FromBody] UpdateVaccineCombo rq, int id)
        {
            var combo = await _vaccineComboService.UpdateVaccineCombo(rq, id);
            return Ok(combo);
        }

        [HttpDelete("deleteVaccineCombo{id}")]
        public async Task<IActionResult> DeleteVaccineCombo(int id)
        {
            var result = await _vaccineComboService.DeleteVaccineCombo(id);
            return Ok(result);
        }
    }
}
