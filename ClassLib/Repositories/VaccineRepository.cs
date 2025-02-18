using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class VaccineRepository
    {
        private readonly DbSwpVaccineTracking2Context _context;

        public VaccineRepository(DbSwpVaccineTracking2Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Vaccine>> GetAllVaccines()
        {
            return await _context.Vaccines.ToListAsync();
        }
    }
}
