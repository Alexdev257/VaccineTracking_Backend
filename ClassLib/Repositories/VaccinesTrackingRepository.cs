using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class VaccinesTrackingRepository
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;
        public VaccinesTrackingRepository(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context;
        }

        // For admin
        public async Task<List<VaccinesTracking>> GetVaccinesTrackingAsync()
        {
            return await _context.VaccinesTrackings
                                    .Include(vt => vt.User)
                                    .Include(vt => vt.Child)
                                    .Include(vt => vt.Vaccine)
                                    .ToListAsync()!;
        }


        // For user
        public async Task<List<VaccinesTracking>> GetVaccinesTrackingByParentIdAsync(int id)
        {
            return await _context.VaccinesTrackings.Where(vt => vt.UserId == id)
                                    .Include(vt => vt.User)
                                    .Include(vt => vt.Child)
                                    .Include(vt => vt.Vaccine)
                                    .Where(x => x.Status == "Schedule" || x.Status == "Pending")
                                    .ToListAsync()!;
        }

        public async Task<List<VaccinesTracking>> GetVaccinesTrackingByBookingID(int bookingID) => await _context.VaccinesTrackings.Where(vt => vt.BookingId == bookingID).ToListAsync();

        public async Task<VaccinesTracking?> GetVaccinesTrackingByIdAsync(int id)
        {
            return await _context.VaccinesTrackings.Where(vt => vt.Id == id)
                                    .Include(vt => vt.Vaccine)
                                    .Include(vt => vt.User)
                                    .FirstOrDefaultAsync();
        }

        // Soft delete by bookingID
        public async Task<int> SoftDeleteByBookingID(int id)
        {
            var searchingResult = await GetVaccinesTrackingByBookingID(id);

            if (searchingResult == null || !searchingResult.Any())
            {
                return 0;
            }

            searchingResult.ForEach(b => b.IsDeleted = true);

            await _context.SaveChangesAsync();

            return searchingResult.Count;
        }


        public async Task<VaccinesTracking> AddVaccinesTrackingAsync(VaccinesTracking vaccinesTracking)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.VaccinesTrackings.Add(vaccinesTracking);
                await _context.SaveChangesAsync(); // Ensure it's saved

                transaction.Commit();
                return vaccinesTracking; // Return the fully saved entity
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                transaction.Rollback();
                return null!;
            }
        }

        public async Task<VaccinesTracking> UpdateVaccinesTrackingAsync(VaccinesTracking vaccinesTracking)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.VaccinesTrackings.Update(vaccinesTracking);
                await _context.SaveChangesAsync();

                transaction.Commit();
                return vaccinesTracking;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                transaction.Rollback();
                return null!;
            }
        }

        public async Task<VaccinesTracking?> GetVaccinesTrackingByPreviousVaccination(int previousVaccination)
        {
            return await _context.VaccinesTrackings
                                    .Where(vt => vt.PreviousVaccination == previousVaccination)
                                    .FirstOrDefaultAsync()!;
        }


        //Alex5
        public async Task<List<VaccinesTracking>> GetUpComingVaccinations(DateTime today)
        {
            return await _context.VaccinesTrackings
                                    .Where(vt => vt.MinimumIntervalDate.HasValue && vt.MinimumIntervalDate.Value.Date == today.AddDays(1).Date)
                                    .Include(vt => vt.Vaccine)
                                    .Include(vt => vt.Child)
                                    .ToListAsync();
        }

        public async Task<List<VaccinesTracking>> GetUpComingVaccinations1(int id)
        {
            return await _context.VaccinesTrackings
                                    .Where(vt => vt.Id == id)
                                    .Include(vt => vt.Vaccine)
                                    .Include(vt => vt.Child)
                                    .ToListAsync();
        }
    }
}