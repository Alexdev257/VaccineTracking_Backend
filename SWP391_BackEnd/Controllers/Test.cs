using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
//using ClassLib.Job;
using ClassLib.Models;
using ClassLib.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassLib.Helpers;

namespace SWP391_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Test : ControllerBase
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;
        //private readonly VaccineTrackingReminderHangfire _reminderJob;
        private readonly VaccinesTrackingRepository _vaccinesTrackingRepository;
        //public Test(DbSwpVaccineTrackingFinalContext context, VaccineTrackingReminderHangfire reminderJob, VaccinesTrackingRepository vaccinesTrackingRepository)
        //{
        //    _context = context;
        //    _reminderJob = reminderJob;
        //    _vaccinesTrackingRepository = vaccinesTrackingRepository;
        //}
        public Test(DbSwpVaccineTrackingFinalContext context, VaccinesTrackingRepository vaccinesTrackingRepository)
        {
            _context = context;
            _vaccinesTrackingRepository = vaccinesTrackingRepository;
        }

        [HttpGet("{id}")]
        public async Task<List<int>> GetListByID([FromRoute] int id) {
            return (await _context.VaccinesCombos.Include(v => v.Vaccines).Where(vc => vc.Id == id).SelectMany(vc => vc.Vaccines).ToListAsync()).Select(v => v.Id).ToList();
        }

        //[HttpPost("test-reminder")]
        //public async Task<IActionResult> test()
        //{
        //    bool rs = await _reminderJob.SendVaccineReminder();
        //    return rs ? Ok("success send") : BadRequest("Failed send");
        //}

        [HttpGet]
        public async Task<IActionResult> getUpcoming()
        {
            var aa = ClassLib.Helpers.TimeProvider.GetVietnamNow();
            var rs = await _vaccinesTrackingRepository.GetUpComingVaccinations(aa);
            //var rs = await _vaccinesTrackingRepository.GetUpComingVaccinations1(aa);
            return Ok(rs);
        }

        //[HttpGet]
        //public async Task<IActionResult> getUpcoming1()
        //{
        //    var aa = ClassLib.Helpers.TimeProvider.GetVietnamNow();
        //    //var rs = await _vaccinesTrackingRepository.GetUpComingVaccinations(aa);
        //    var rs = await _vaccinesTrackingRepository.GetUpComingVaccinations2(aa);
        //    return Ok(rs);
        //}

        //[HttpPost]
        //public async Task<IActionResult> getUpcoming2()
        //{
        //    var aa = ClassLib.Helpers.TimeProvider.GetVietnamNow();
        //    //var rs = await _vaccinesTrackingRepository.GetUpComingVaccinations(aa);
        //    var rs = await _vaccinesTrackingRepository.compare(aa);
        //    return Ok(rs);
        //}

        [HttpGet("abc")]
        public async Task<IActionResult> getDeadline()
        {
            var aa = ClassLib.Helpers.TimeProvider.GetVietnamNow();
            var rs = await _vaccinesTrackingRepository.GetDeadlineVaccinations(aa);
            //var rs = await _vaccinesTrackingRepository.GetUpComingVaccinations1(aa);
            return Ok(rs);
        }
    }


}