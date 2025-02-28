using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.VaccineTracking;
using ClassLib.Service;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _vaccinesTrackingService.GetVaccinesTrackingAsync());
        }

        [HttpGet("get-by-parent-id/{id}")]
        public async Task<IActionResult> GetByParentId(int id)
        {
            return Ok(await _vaccinesTrackingService.GetVaccinesTrackingByParentIdAsync(id));
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _vaccinesTrackingService.GetVaccinesTrackingByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddVaccinesTracking([FromBody] Test test)
        {
            return Ok(await _vaccinesTrackingService.AddVaccinesToVaccinesTrackingAsync(test.AddVaccinesTrackingRequest, test.vaccineIds, test.childrenIds));
        }
    }
}