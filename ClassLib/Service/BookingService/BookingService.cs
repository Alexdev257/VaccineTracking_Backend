using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.Booking;
using ClassLib.DTO.Payment;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories;

namespace ClassLib.Service
{
    public class BookingService
    {
        private readonly BookingRepository _bookingRepository;
        private readonly UserRepository _userRepository;
        public BookingService(BookingRepository bookingRepository, UserRepository userRepository)
        {
            this._bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<List<Booking>?> GetByQuerry(BookingQuerryObject query)
        {
            return await _bookingRepository.GetByQuerry(query);
        }

        public async Task<OrderInfoModel?> AddBooking(AddBooking addBooking)
        {
            var booking = await _bookingRepository.AddBooking(addBooking);
            var user = await _userRepository.getUserByIdAsync(addBooking.ParentId);

            return new OrderInfoModel
            {
                GuestName = user?.Name,
                GuestEmail = user?.Gmail,
                GuestPhone = user?.PhoneNumber,
                BookingID = booking.Id.ToString(),
                OrderId = booking.Id.ToString(),
                OrderDescription = booking.AdvisoryDetails,
                Amount = addBooking.TotalPrice
            };
        }

        public async Task<Booking?> UpdateBookingStatus(string bookingId, string msg)
        {
            return await _bookingRepository.UpdateBooking(bookingId, msg);
        }
    }
}