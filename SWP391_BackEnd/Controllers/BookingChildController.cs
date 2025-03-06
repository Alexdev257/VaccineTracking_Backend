using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SWP391_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class BookingChildController : ControllerBase
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;

        public BookingChildController(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Booking>>> Get()
        {
            return await _context.Bookings.Include(b => b.Children).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Booking>> AddBooking(int bookingID, int childID){
            if( await _context.Bookings.FindAsync(bookingID) == null || await _context.Children.FindAsync(childID) == null){
                return BadRequest();
            }
            var booking = _context.Bookings.FindAsync(bookingID);
            booking.Result.Children.Add(_context.Children.FindAsync(childID).Result);
            await _context.SaveChangesAsync();
            return Ok(booking);
        }
    }
}