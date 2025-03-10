using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class BookingIdVaccineIdReponsitory
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;

        public BookingIdVaccineIdReponsitory(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Add(Booking booking, List<int> vaccineId)
        {
            try
            {
                foreach (var id in vaccineId)
                {
                    var vaccine = await _context.Vaccines.FindAsync(id);
                    if (vaccine == null)
                    {
                        return false;
                    }
                    booking.Vaccines.Add(vaccine);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<bool> ClearAndAdd(Booking booking, List<int> vaccineId)
        {
            try
            {
                // Clear
                var result = await _context.Bookings
                            .Include(b => b.Vaccines)
                            .Where(b => b.Id == booking.Id)
                            .ToListAsync();
                foreach (var b in result)
                {
                    b.Vaccines.Clear();
                }
                await _context.SaveChangesAsync();

                // Add
                foreach (var id in vaccineId)
                {
                    var vaccine = await _context.Vaccines.FindAsync(id);
                    if (vaccine == null)
                    {
                        return false;
                    }
                    booking.Vaccines.Add(vaccine);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}