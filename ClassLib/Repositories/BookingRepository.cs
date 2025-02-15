using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.Models;

using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class BookingRepository
    {
        private readonly DbSwpVaccineTracking2Context _context;
        public BookingRepository(DbSwpVaccineTracking2Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Booking>> getAll()
        {
            return await _context.Bookings.ToListAsync();
        }

        public async Task<List<Booking>> getBookingByParentId(int userId)
        {
            return await _context.Bookings.Where(b => b.ParentId == userId).ToListAsync();
        }
    }
}