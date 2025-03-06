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
        private readonly DbSwpVaccineTrackingFinalContext _context;

        public VaccineRepository(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Vaccine>> GetAllVaccines()
        {
            return await _context.Vaccines.Include(v => v.Address).Where(v => v.IsDeleted == false).ToListAsync();
        }

        public async Task<List<Vaccine>> GetAllVaccinesAdmin()
        {
            return await _context.Vaccines.Include(v => v.Address).ToListAsync();
        }

        public async Task<Vaccine?> GetById(int id)
        {
            //return await _context.Set<Vaccine>().FindAsync(id);
            return await _context.Vaccines.FirstOrDefaultAsync(v => v.Id == id && v.IsDeleted == false);
        }

        public async Task<Vaccine?> GetByIdAdmin(int id)
        {
            //return await _context.Set<Vaccine>().FindAsync(id);
            return await _context.Vaccines.FirstOrDefaultAsync(v => v.Id == id);
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
            return await _context.Vaccines
                .Where(v => age > v.SuggestAgeMin && age < v.SuggestAgeMax && v.IsDeleted == false)
                .ToListAsync();
        }
        
        public async Task<List<Vaccine>> GetVaccinesByAgeAdmin(int age)
        {
            return await _context.Vaccines
                .Where(v => age > v.SuggestAgeMin && age < v.SuggestAgeMax)
                .ToListAsync();
        }

        


        //TieHung
        public async Task<bool> DecreseQuantityVaccines(Vaccine vaccine,int amount){

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
    }
}
