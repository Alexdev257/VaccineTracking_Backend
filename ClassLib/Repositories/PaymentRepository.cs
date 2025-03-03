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
        private readonly DbSwpVaccineTrackingFinalContext _context;
        public PaymentRepository(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context;
        }

        public async Task<List<Payment>> GetAllAsync() => await _context.Payments.ToListAsync();

        public async Task<Payment> GetByIDAsync(int id) => await _context.Payments.FindAsync(id);

        public async Task<Payment> AddPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}