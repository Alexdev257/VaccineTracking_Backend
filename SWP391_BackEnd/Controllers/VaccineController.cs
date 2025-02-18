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

        [HttpGet]
        public async Task<IActionResult> GetVaccines()
        {
            var vaccines = await _vaccineService.GetAllVaccines();
            if (vaccines == null || vaccines.Count == 0)
            {
                return NotFound("No vaccines found.");
            }
            return Ok(vaccines);
        }
    }
}
