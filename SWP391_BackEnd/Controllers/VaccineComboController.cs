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
            //try
            //{
                var combos = await _vaccineComboService.GetAllVaccineCombo();
                if (combos == null || combos.Count == 0)
                {
                    return NotFound("No combos found.");
                }
                return Ok(combos);
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(500, $"Internal Server Error: {ex.Message}");
            //}
        }

        [HttpGet("getVaccineComboById/{id}")]
        public async Task<IActionResult> GetVaccineCombo(int id)
        {
            try
            {
                var combo = await _vaccineComboService.GetVaccineComboById(id);
                if (combo == null)
                {
                    return NotFound("No combo found.");
                }
                return Ok(combo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("createVaccineCombo")]
        public async Task<IActionResult> CreateVaccine([FromBody] CreateVaccineCombo rq)
        {
            try
            {
                var combo = await _vaccineComboService.CreateVaccineCombo(rq);
                return Ok(combo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("updateVaccineComboById/{id}")]
        public async Task<IActionResult> UpdateVaccine([FromBody] UpdateVaccineCombo rq, int id)
        {
            try
            {
                var combo = await _vaccineComboService.UpdateVaccineCombo(rq, id);
                return Ok(combo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("addVaccine/{id}")]
        public async Task<IActionResult> AddVaccine([FromBody] AddVaccineIntoCombo rq, int id)
        {
            try
            {
                var combo = await _vaccineComboService.AddVaccine(rq, id);
                return Ok(combo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("deleteVaccineCombo/{id}")]
        public async Task<IActionResult> DeleteVaccineCombo(int id)
        {
            try
            {
                var result = await _vaccineComboService.DeleteVaccineCombo(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}

