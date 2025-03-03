using ClassLib.DTO.Vaccine;
using ClassLib.DTO.VaccineCombo;
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
        private readonly DbSwpVaccineTrackingFinalContext _context;

        public VaccineComboRepository(DbSwpVaccineTrackingFinalContext context)
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
            return await _context.VaccinesCombos
                .Include(vc => vc.Vaccines)  // Thêm Include ở đây
                .ToListAsync();
        }

        public async Task<VaccinesCombo?> GetById(int id)
        {
            return await _context.VaccinesCombos.Include(c => c.Vaccines).Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<VaccinesCombo> UpdateVaccine(VaccinesCombo currentCombo, VaccinesCombo updateCombo)
        {
            _context.Entry(currentCombo).CurrentValues.SetValues(updateCombo);
            await _context.SaveChangesAsync();
            return currentCombo;
        }

        public async Task<VaccinesCombo?> UpdateVaccineWithID(int comboID, VaccinesCombo updateCombo)
        {
            var currentCombo = await _context.Set<VaccinesCombo>().FindAsync(comboID);
            _context.Entry(currentCombo).CurrentValues.SetValues(updateCombo);
            await _context.SaveChangesAsync();
            return await _context.VaccinesCombos
                .Where(vc => vc.Id == comboID)
                .Select(vc => new VaccinesCombo()
                {
                    Id = vc.Id,
                    ComboName = vc.ComboName,
                    Vaccines = vc.Vaccines.Select(v => new Vaccine()
                    {
                        Name = v.Name,
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        // TieHung23
        public async Task<List<Vaccine>> GetAllVaccineInVaccinesComboByID(int id)
        {
            return await _context.VaccinesCombos.Include(v => v.Vaccines).Where(vc => vc.Id == id).SelectMany(vc => vc.Vaccines).ToListAsync();
        }
    }
}
