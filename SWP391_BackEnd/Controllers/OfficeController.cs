using ClassLib.Service;
using ClassLib.Service.OfficeService;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeController : ControllerBase
    {
        private readonly OfficeService _officeService;
        public OfficeController(OfficeService officeService)
        {
            _officeService = officeService;
        }
        [HttpGet("/offices")]
       public async Task<IActionResult> GetAllOffice()
        {
            try
            {
                var office = await _officeService.GetAllOffice();
                return Ok(office);
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
