using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class VaccineComboRepository
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;

        public VaccineComboRepository(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<VaccinesCombo?> GetDetailVaccineComboById(int id)
        {
            return await _context.VaccinesCombos.Include(c => c.Vaccines).FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);
        }

        //Alex5
        public async Task<VaccinesCombo?> GetDetailVaccineComboByIdAdmin(int id)
        {
            return await _context.VaccinesCombos.Include(c => c.Vaccines).FirstOrDefaultAsync(c => c.Id == id);
        }
        
        public async Task<VaccinesCombo> CreateVaccine(VaccinesCombo newCombo)
        {
            _context.Add(newCombo);
            await _context.SaveChangesAsync();
            return newCombo;
        }

        public async Task<bool> DeleteVaccineCombo(VaccinesCombo currentCombo)
        {
            currentCombo.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<VaccinesCombo>> GetAllVaccineCombo()
        {
            return await _context.VaccinesCombos
                .Include(vc => vc.Vaccines)
                .Where(vc => vc.IsDeleted == false) // 
                .ToListAsync();
        }

        //Alex5
        public async Task<List<VaccinesCombo>> GetAllVaccineComboAdmin()
        {
            return await _context.VaccinesCombos
                .Include(vc => vc.Vaccines)
                .ToListAsync();
        }

        public async Task<VaccinesCombo?> GetById(int id)
        {
            return await _context.VaccinesCombos.Include(c => c.Vaccines).Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        //Alex5
        public async Task<VaccinesCombo?> GetByIdAdmin(int id)
        {
            return await _context.VaccinesCombos.Include(c => c.Vaccines).Where(c => c.Id == id && c.IsDeleted == false).FirstOrDefaultAsync();
        }

        public async Task<VaccinesCombo> UpdateVaccine(VaccinesCombo currentCombo, VaccinesCombo updateCombo)
        {
            _context.Entry(currentCombo).CurrentValues.SetValues(updateCombo);
            await _context.SaveChangesAsync();
            return currentCombo;
        }

        //Alex5
        public async Task<bool> UpdateCombo(VaccinesCombo combo)
        {
            _context.VaccinesCombos.Update(combo);
            return await _context.SaveChangesAsync() > 0;
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

        public async Task<VaccinesCombo?> RemoveVaccineFromCombo(int id, List<int> removedVaccineIds)
        {
            var combo = await _context.VaccinesCombos
               .Include(vc => vc.Vaccines)
               .Where(vc => vc.Id == id).FirstOrDefaultAsync();

            var existingVaccineIds = combo.Vaccines.Select(v => v.Id).ToList();
            var invalidVaccineIds = removedVaccineIds.Except(existingVaccineIds).ToList();
            if (invalidVaccineIds.Any())
            {
                return null;

            }

            combo.Vaccines = combo.Vaccines
                .Where(v => !removedVaccineIds.Contains(v.Id))
                .ToList();

            await _context.SaveChangesAsync();

            return combo;
        }

        //TieHung23
        public Task<decimal> SumMoneyOfComboList(List<VaccinesCombo> list)
        {
            decimal total = 0;

            foreach (var vt in list)
            {
                total += vt.FinalPrice;
            }

            return Task.FromResult(total);
        }
    }
}
