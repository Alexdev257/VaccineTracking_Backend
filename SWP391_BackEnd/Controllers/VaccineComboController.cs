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

        [HttpGet("get-all-vaccine-combo-admin")]
        public async Task<IActionResult> GetVaccineCombosAdmin()
        {
            //try
            //{
            var combos = await _vaccineComboService.GetAllVaccineComboAdmin();
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

        [HttpGet("get-vaccine-combo-detail/{id}")]
        public async Task<IActionResult> GetVaccineComBoDetailById(int id)
        {
            try
            {
                var combo = await _vaccineComboService.GetDetailVaccineComboByIdAsync(id);
                return Ok(combo);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("get-vaccine-combo-detail-admin/{id}")]
        public async Task<IActionResult> GetVaccineComBoDetailByIdAdmin(int id)
        {
            try
            {
                var combo = await _vaccineComboService.GetDetailVaccineComboByIdAsyncAdmin(id);
                return Ok(combo);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
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

        [HttpPatch("soft-delete-combo/{id}")]
        public async Task<IActionResult> SoftDeleteCombo(int id)
        {
            try
            {
                var rs = await _vaccineComboService.SoftDeleteVaccineCombo(id);
                return Ok("Delete successfully");
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            //catch(Exception e)
            //{
            //    return StatusCode(500, "Something went wrong");
            //}
        }

        [HttpPut("removeVaccineFromCombo/{id}")]
        public async Task<IActionResult> RemoveVaccine([FromBody] AddVaccineIntoCombo rq, int id)// dung chung dto voi addvacineintocombo
        {
            try
            {
                var combo = await _vaccineComboService.RemoveVaccine(rq, id);
                return Ok(combo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}

