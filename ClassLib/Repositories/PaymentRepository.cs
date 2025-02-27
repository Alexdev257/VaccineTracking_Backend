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

        public async Task<Payment> getById(int id) => await _context.Payments.FindAsync(id);

        public async Task<Payment> create(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}