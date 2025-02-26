//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using ClassLib.Models;
//using Microsoft.EntityFrameworkCore;

//namespace ClassLib.Repositories
//{
//    public class BookingIdVaccineIdReponsitory
//    {
//        private readonly DbSwpVaccineTrackingContext _context;

//        public BookingIdVaccineIdReponsitory(DbSwpVaccineTrackingContext context)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//        }

//        public async Task<List<BookingIdVaccineId>> GetAll()
//        {
//            return await _context.BookingIdVaccineIds.ToListAsync();
//        }

//        public async Task<BookingIdVaccineId?> GetById(int id)
//        {
//            return await _context.BookingIdVaccineIds.FirstOrDefaultAsync(x => x.BookingId == id);
//        }

//        public async Task<bool> Add(Booking booking, int[] vaccineId)
//        {
//            foreach (var item in vaccineId)
//            {
//                var bookingIdVaccineId = new BookingIdVaccineId
//                {
//                    BookingId = booking.Id,
//                    VaccineId = item
//                };
//                _context.BookingIdVaccineIds.Add(bookingIdVaccineId);
//            }
//            await _context.SaveChangesAsync();
//            return true;
//        }
//    }
//}