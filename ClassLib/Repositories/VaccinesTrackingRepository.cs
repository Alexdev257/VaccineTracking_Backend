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
        private readonly DbSwpVaccineTrackingContext _context;
        public VaccinesTrackingRepository(DbSwpVaccineTrackingContext context)
        {
            _context = context;
        }

        public async Task<List<VaccinesTracking>> GetVaccinesTrackingAsync()
        {
            return await _context.VaccinesTrackings
                                    .Include(vt => vt.User)
                                    .Include(vt => vt.Child)
                                    .Include(vt => vt.Vaccine)
                                    .ToListAsync()!;
        }

        public async Task<List<VaccinesTracking>> GetVaccinesTrackingByParentIdAsync(int id)
        {
            return await _context.VaccinesTrackings.Where(vt => vt.UserId == id)
                                    .Include(vt => vt.User)
                                    .Include(vt => vt.Child)
                                    .Include(vt => vt.Vaccine)
                                    .ToListAsync()!;
        }

        public async Task<VaccinesTracking> GetVaccinesTrackingByIdAsync(int id)
        {
            return await _context.VaccinesTrackings.FindAsync(id);
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
                return null;
            }
        }

        public async Task<VaccinesTracking> UpdateVaccinesTrackingAsync(VaccinesTracking vaccinesTracking, string status, string reaction)
        {
            vaccinesTracking.Status = status;
            vaccinesTracking.Reaction = reaction;
            _context.Entry(vaccinesTracking).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return vaccinesTracking;
        }
    }
}