using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.VaccineCombo;
using ClassLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

        public async Task<bool> Clear(Booking booking)
        {
            try
            {
                var result = await _context.Bookings
                            .Include(b => b.Combos) // Ensure EF Core loads the related data
                            .Where(b => b.Id == booking.Id)
                            .ToListAsync();
                foreach (var b in result)
                {
                    b.Combos.Clear();
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
                // Clear
                var result = await _context.Bookings
                            .Include(b => b.Combos) // Ensure EF Core loads the related data
                            .Where(b => b.Id == booking.Id)
                            .ToListAsync();
                foreach (var b in result)
                {
                    b.Combos.Clear();
                }
                await _context.SaveChangesAsync();

                // Add
                if (!comboId.IsNullOrEmpty())
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
                }
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