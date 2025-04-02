// 
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
            booking.Result!.Children.Add(_context.Children.FindAsync(childID).Result!);
            await _context.SaveChangesAsync();
            return Ok(booking);
        }
    }
}
// This code defines a BookingChildController class that handles HTTP requests related to booking children in a vaccine tracking system.
// It includes methods to get all bookings and to add a child to a specific booking. The controller interacts with a database context (DbSwpVaccineTrackingFinalContext) to perform these operations. The Get method retrieves all bookings along with their associated children, while the AddBooking method adds a child to a specified booking if both exist in the database. If either the booking or child does not exist, it returns a BadRequest response.
// wfewfwfwfwef