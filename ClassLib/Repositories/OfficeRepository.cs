using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class OfficeRepository
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;
        public OfficeRepository(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context;
        }
        public async Task<List<Office>> GetAll()
        {
            return await _context.Offices.ToListAsync();
        }

    }
}
