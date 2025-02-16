using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories;

namespace ClassLib.Service
{
    public class BookingService
    {
         private readonly BookingRepository _bookingRepository;
        public BookingService(BookingRepository bookingRepository)
        {
            this._bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        }

        public async Task<List<Booking>?> GetByQuerry(BookingQuerryObject query)
        {
            return await _bookingRepository.GetByQuerry(query);
        }
    }
}