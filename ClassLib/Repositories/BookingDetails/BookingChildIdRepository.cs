using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories.BookingDetails
{
    public class BookingChildIdRepository
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;

        public BookingChildIdRepository(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Add(Booking booking, List<int> childId)
        {
            try
            {
                foreach (var id in childId)
                {
                    var child = await _context.Children.FindAsync(id);
                    if (child == null)
                    {
                        return false;
                    }
                    booking.Children.Add(child);
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