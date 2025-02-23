using ClassLib.DTO.Vaccine;
using ClassLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.Repositories
{
    public class VaccineComboRepository
    {
        private readonly DbSwpVaccineTrackingContext _context;

        public VaccineComboRepository(DbSwpVaccineTrackingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<VaccinesCombo> CreateVaccine(VaccinesCombo newCombo)
        {
            _context.Add(newCombo);
            await _context.SaveChangesAsync();
            return newCombo;
        }

        public async Task<bool> DeleteVaccineCombo(VaccinesCombo currentCombo)
        {
            _context.Set<VaccinesCombo>().Remove(currentCombo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<VaccinesCombo>> GetAllVaccineCombo()
        {
            return await _context.VaccinesCombos.ToListAsync();
        }

        public async Task<VaccinesCombo?> GetById(int id)
        {
            return await _context.Set<VaccinesCombo>().FindAsync(id);
        }

        public async Task<VaccinesCombo> UpdateVaccine(VaccinesCombo currentCombo, VaccinesCombo updateCombo)
        {
            _context.Entry(currentCombo).CurrentValues.SetValues(updateCombo);
            await _context.SaveChangesAsync();
            return currentCombo;
        }
    }
}
