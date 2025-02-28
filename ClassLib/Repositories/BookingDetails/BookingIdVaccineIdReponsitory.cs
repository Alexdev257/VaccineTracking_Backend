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
        private readonly DbSwpVaccineTrackingContext _context;

        public BookingIdVaccineIdReponsitory(DbSwpVaccineTrackingContext context)
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
    }
}