using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class PaymentRepository
    {
        private readonly DbSwpVaccineTrackingContext _context;
        public PaymentRepository(DbSwpVaccineTrackingContext context)
        {
            _context = context;
        }

        public async Task<List<Payment>> getAll() => await _context.Payments.ToListAsync();

        //public async Task<Payment?> getByID(int id) => await _context.Payments.FirstOrDefaultAsync( i => i.Id == id);
    }
}