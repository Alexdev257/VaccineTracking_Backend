using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.DTO.Vaccine;
using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class VaccineRepository
    {
        private readonly DbSwpVaccineTrackingContext _context;

        public VaccineRepository(DbSwpVaccineTrackingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Vaccine>> GetAllVaccines()
        {
            return await _context.Vaccines.ToListAsync();
        }

        public async Task<Vaccine?> GetById(int id)
        {
            return await _context.Set<Vaccine>().FindAsync(id);
        }

        public async Task<Vaccine> CreateVaccine(Vaccine newVaccine)
        {
            _context.Add(newVaccine);
            await _context.SaveChangesAsync();
            return newVaccine;
        }

        public async Task<Vaccine> UpdateVaccine(Vaccine currentVaccine, Vaccine updateVaccine)
        {
            _context.Entry(currentVaccine).CurrentValues.SetValues(updateVaccine);
            await _context.SaveChangesAsync();
            return currentVaccine;
        }

        public async Task<bool> DeleteVaccine(Vaccine currentVaccine)
        {
            _context.Set<Vaccine>().Remove(currentVaccine);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
