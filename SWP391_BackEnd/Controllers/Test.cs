using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.Job;
using ClassLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SWP391_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Test : ControllerBase
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;
        private readonly VaccineTrackingReminderJob _reminderJob;
        public Test(DbSwpVaccineTrackingFinalContext context, VaccineTrackingReminderJob reminderJob)
        {
            _context = context;
            _reminderJob = reminderJob;
        }

        [HttpGet("{id}")]
        public async Task<List<int>> GetListByID([FromRoute] int id){
            return (await _context.VaccinesCombos.Include(v => v.Vaccines).Where(vc => vc.Id == id).SelectMany(vc => vc.Vaccines).ToListAsync()).Select(v => v.Id).ToList();
        }

        [HttpPost("test-reminder")]
        public async Task<IActionResult> test()
        {
            bool rs = await _reminderJob.SendVaccineReminder();
            return rs ? Ok("success send") : BadRequest("Failed send");
        }
    }
}