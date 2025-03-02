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

        public async Task<List<Vaccines>> GetAllVaccines()
        {
            return await _context.Vaccines.Include(v => v.Address).ToListAsync();
        }

        public async Task<Vaccines?> GetById(int id)
        {
            return await _context.Set<Vaccines>().FindAsync(id);
        }

        public async Task<Vaccines> CreateVaccine(Vaccines newVaccine)
        {
            _context.Add(newVaccine);
            await _context.SaveChangesAsync();
            return newVaccine;
        }

        public async Task<Vaccines> UpdateVaccine(Vaccines currentVaccine, Vaccines updateVaccine)
        {
            _context.Entry(currentVaccine).CurrentValues.SetValues(updateVaccine);
            await _context.SaveChangesAsync();
            return currentVaccine;
        }

        public async Task<bool> DeleteVaccine(Vaccines currentVaccine)
        {
            _context.Set<Vaccines>().Remove(currentVaccine);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Vaccines>> GetVaccinesByAge(int age)
        {
            return await _context.Vaccines
                .Where(v => age > v.SuggestAgeMin && age < v.SuggestAgeMax)
                .ToListAsync();
        }


        //TieHung
        public async Task<bool> DecreseQuantityVaccines(Vaccines vaccine,int amount){

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
