//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using ClassLib.Models;
//using Microsoft.EntityFrameworkCore;

//namespace ClassLib.Repositories
//{
//    public class BookingComboIdReponsitory
//    {
//        private readonly DbSwpVaccineTrackingContext _context;

//        public BookingComboIdReponsitory(DbSwpVaccineTrackingContext context)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//        }

//        public async Task<List<BookingComboId>> GetAll()
//        {
//            return await _context.BookingComboIds.ToListAsync();
//        }

//        public async Task<BookingComboId?> GetById(int id)
//        {
//            return await _context.BookingComboIds.FirstOrDefaultAsync(x => x.BookingId == id);
//        }

//        public async Task<bool> Add(Booking booking, int[] comboId)
//        {
//            foreach (var item in comboId)
//            {
//                var bookingComboId = new BookingComboId
//                {
//                    BookingId = booking.Id,
//                    ComboId = item
//                };
//                _context.BookingComboIds.Add(bookingComboId);
//            }
//            await _context.SaveChangesAsync();
//            return true;
//        }
//    }
//}