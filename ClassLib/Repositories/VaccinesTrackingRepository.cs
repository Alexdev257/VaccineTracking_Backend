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
            return _context.VaccinesTrackings.ToList()!;
        }

        public async Task<List<VaccinesTracking>> GetVaccinesTrackingByParentIdAsync(int id)
        {
            return _context.VaccinesTrackings.Where(vt => vt.UserId == id).ToList()!;
        }
    }
}