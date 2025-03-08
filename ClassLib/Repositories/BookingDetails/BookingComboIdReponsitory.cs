using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class BookingComboIdReponsitory
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;

        public BookingComboIdReponsitory(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Add(Booking booking, List<int> comboId)
        {
            try
            {
                foreach (var id in comboId)
                {
                    var combo = await _context.VaccinesCombos.FindAsync(id);
                    if (combo == null)
                    {
                        return false;
                    }
                    booking.Combos.Add(combo);
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

        public async Task<bool> ClearAndAdd(Booking booking, List<int> comboId)
        {
            try
            {
                booking.Combos.Clear();
                return await Add(booking, comboId);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}