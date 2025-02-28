using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.Booking;
using ClassLib.Enum;
using ClassLib.Helpers;
using ClassLib.Service;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Model;
using PaymentAPI.Services;

namespace SWP391_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
        private readonly HttpClient _httpClient;
        public BookingController(BookingService bookingService, HttpClient httpClient)
        {
            _bookingService = bookingService;
            _httpClient = httpClient;
        }


        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] BookingQuerryObject collection)
        {
            return Ok(await _bookingService.GetByQuerry(collection));
        }


        [HttpPost("add-booking")]
        public async Task<IActionResult> AddBooking(AddBooking addBooking)
        {
            return Ok(await _bookingService.AddBooking(addBooking));
        }
    }
}