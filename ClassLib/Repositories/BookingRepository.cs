using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.Helpers;
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

        public async Task<List<Booking>?> GetByQuerry(BookingQuerryObject bookingQuerryObject)
        {
            var booking = _context.Bookings
                          .Include(x => x.Parent)
                          .AsQueryable();

            if (bookingQuerryObject.Id.HasValue)
            {
                booking = booking.Where(x => x.Id == bookingQuerryObject.Id);
            }
            if (bookingQuerryObject.ParentId.HasValue)
            {
                booking = booking.Where(x => x.ParentId == bookingQuerryObject.ParentId);
            }
            if (bookingQuerryObject.Status != null)
            {
                booking = booking.Where(x => x.Status == bookingQuerryObject.Status);
            }
            if (bookingQuerryObject.CreateDate.HasValue)
            {
                booking = booking.Where(x => x.CreatedAt == bookingQuerryObject.CreateDate);
            }
            if (bookingQuerryObject.PhoneNumber != null)
            {
                booking = booking.Where(x => x.Parent.PhoneNumber == bookingQuerryObject.PhoneNumber);
            }
            if (bookingQuerryObject.orderBy != null)
            {
                switch (bookingQuerryObject.orderBy)
                {
                    case "Id":
                        booking = bookingQuerryObject.isDescending ? booking.OrderByDescending(x => x.Id) : booking.OrderBy(x => x.Id);
                        break;
                    case "ParentId":
                        booking = bookingQuerryObject.isDescending ? booking.OrderByDescending(x => x.ParentId) : booking.OrderBy(x => x.ParentId);
                        break;
                    case "TotalPrice":
                        booking = bookingQuerryObject.isDescending ? booking.OrderByDescending(x => x.TotalPrice) : booking.OrderBy(x => x.TotalPrice);
                        break;
                    case "CreatedAt":
                        booking = bookingQuerryObject.isDescending ? booking.OrderByDescending(x => x.CreatedAt) : booking.OrderBy(x => x.CreatedAt);
                        break;
                    case "ArrivedAt":
                        booking = bookingQuerryObject.isDescending ? booking.OrderByDescending(x => x.ArrivedAt) : booking.OrderBy(x => x.ArrivedAt);
                        break;
                    case "Status":
                        booking = bookingQuerryObject.isDescending ? booking.OrderByDescending(x => x.Status) : booking.OrderBy(x => x.Status);
                        break;
                }
            }

            return await booking.ToListAsync();
        }

    }
}