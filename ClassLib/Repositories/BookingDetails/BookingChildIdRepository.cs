//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using ClassLib.Models;
//using Microsoft.EntityFrameworkCore;

//namespace ClassLib.Repositories.BookingDetails
//{
//    public class BookingChildIdRepository
//    {
//        private readonly DbSwpVaccineTrackingContext _context;

//        public BookingChildIdRepository(DbSwpVaccineTrackingContext context)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//        }

//        public async Task<List<BookingChildId>> GetAll()
//        {
//            return await _context.BookingChildIds.ToListAsync();
//        }

//        public async Task<BookingChildId?> GetById(int id)
//        {
//            return await _context.BookingChildIds.FirstOrDefaultAsync(x => x.BookingId == id);
//        }

//        public async Task<bool> Add(Booking booking, int[] childId){
//            foreach (var item in childId)
//            {
//                var bookingChildId = new BookingChildId
//                {
//                    BookingId = booking.Id,
//                    ChildId = item
//                };
//                _context.BookingChildIds.Add(bookingChildId);
//            }
//            await _context.SaveChangesAsync();
//            return true;
//        }
//    }
//}