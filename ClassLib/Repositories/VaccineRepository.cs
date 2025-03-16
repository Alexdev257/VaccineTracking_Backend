using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class VaccineRepository
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;

        public VaccineRepository(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Vaccine>> GetAllVaccines()
        {
            return await GetAllVaccineHelper().Where(v => v.IsDeleted == false).ToListAsync();
        }
        public async Task<List<Vaccine>> GetAllVaccinesAdmin()
        {
            return await GetAllVaccineHelper().ToListAsync();
        }

        private IQueryable<Vaccine> GetAllVaccineHelper()
        {
            return  _context.Vaccines.Include(v => v.Address);
        }

        public async Task<Vaccine?>GetById(int id)
        {
            return await GetByIDHelper(id).FirstOrDefaultAsync(v => v.IsDeleted == false);
        }
        public async Task<Vaccine?> GetByIdAdmin(int id)
        {
            return await GetByIDHelper(id).Include(v => v.VacineCombos).FirstOrDefaultAsync();
        }

        private IQueryable<Vaccine> GetByIDHelper(int id)
        {
            return _context.Vaccines.Where(v => v.Id == id);
        }

        public async Task<Vaccine> CreateVaccine(Vaccine newVaccine)
        {
            _context.Add(newVaccine);
            await _context.SaveChangesAsync();
            return newVaccine;
        }

        //public async Task<Vaccine> UpdateVaccine(Vaccine currentVaccine, Vaccine updateVaccine)
        //{
        //    _context.Entry(currentVaccine).CurrentValues.SetValues(updateVaccine);
        //    await _context.SaveChangesAsync();
        //    return currentVaccine;
        //}

        //Alex5
        public async Task<bool> UpdateVaccine(Vaccine vaccine)
        {
            _context.Vaccines.Update(vaccine);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteVaccine(Vaccine vaccine)
        {
            _context.Set<Vaccine>().Remove(vaccine);
            return await _context.SaveChangesAsync() > 0;

        }
        public async Task<List<Vaccine>> GetVaccinesByAge(int age)
        {
            return await GetVaccinesByAgeHelper(age)
                .Where(v => v.IsDeleted == false)
                .ToListAsync();
        }

        public async Task<List<Vaccine>> GetVaccinesByAgeAdmin(int age)
        {
            return await GetVaccinesByAgeHelper(age).ToListAsync(); 
        }

        private IQueryable<Vaccine> GetVaccinesByAgeHelper(int age)
        {
            return _context.Vaccines
               .Where(v => age > v.SuggestAgeMin && age < v.SuggestAgeMax);
        }

        //TieHung
        public async Task<bool> DecreseQuantityVaccines(Vaccine vaccine, int amount)
        {

            using var transcation = _context.Database.BeginTransaction();
            if (vaccine == null) return false;
            try
            {
                vaccine.Quantity -= amount;
                await _context.SaveChangesAsync();
                transcation.Commit();
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                transcation.Rollback();
                return false;
            }
        }
        //TieHung23
        public Task<decimal> SumMoneyOfVaccinesList(List<Vaccine> list)
        {
            decimal total = 0;

            foreach (var vt in list)
            {
                total += vt.Price;
            }

            return Task.FromResult(total);
        }

        //Alex5
        public async Task<List<Vaccine>> GetVaccinesByComboId(int comboId)
        {
            return await _context.Vaccines
                .Where(v => v.VacineCombos.Any(vc => vc.Id == comboId))
                .ToListAsync();
        }

        //Alex5

        public async Task<List<Vaccine>> GetNearlyExpiredVaccine()
        {
            var today = Helpers.TimeProvider.GetVietnamNow();
            return await _context.Vaccines
                .Include(v => v.VacineCombos)
                // == instock de handle truong hop vaccine bi unstock boi admin hoac da cap nhat nearlyoutstock vao ngay hom truoc roi
                .Where(v => (v.TimeExpired.Date <= today.AddDays(3).Date || v.Quantity < v.DoesTimes) && v.Status.ToLower() == "instock".ToLower())
                .ToListAsync();
        }

        //Alex5
        public async Task<List<Vaccine>> GetExpiredVaccine()
        {
            var today = Helpers.TimeProvider.GetVietnamNow();
            return await _context.Vaccines
                .Include(v => v.VacineCombos)
                .Where(v => v.TimeExpired.Date <= today.Date || v.Quantity < v.DoesTimes)
                .ToListAsync();
        }

    }
}
