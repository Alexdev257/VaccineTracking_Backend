using ClassLib.DTO.Booking;
using ClassLib.Helpers;
using ClassLib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        }


        [HttpGet]
        public async Task<IActionResult> Get( [FromQuery] BookingQuerryObject bqo)
        {
            var bookings = await _bookingService.GetByQuerry(bqo);

          if(bookings == null) return NotFound();
          return Ok(bookings);
        }

        [HttpPost]
        public IActionResult AddBooking([FromBody] AddBooking addBooking)
        {
            var booking = _bookingService.AddBooking(addBooking);
            return Ok(booking);
        }
    }
}
