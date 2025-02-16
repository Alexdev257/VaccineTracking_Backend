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
        public IActionResult GetByQuerry([FromQuery] BookingQuerryObject query)
        {
            var bookings = _bookingService.GetByQuerry(query);
            return Ok(bookings);
        }

    }
}
