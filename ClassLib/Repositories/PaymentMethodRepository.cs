using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class PaymentMethodRepository
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;
        public PaymentMethodRepository(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context;
        }
        public async Task<List<PaymentMethod>> getAll() => await _context.PaymentMethods.ToListAsync();

        public async Task<PaymentMethod> getPaymentMethodById(int id) => await _context.PaymentMethods.FirstOrDefaultAsync(x => x.Id == id);
        public async Task<PaymentMethod> getPaymentMethodByName(string name) => await _context.PaymentMethods.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());     
        public async Task<PaymentMethod> addPaymentMethod(PaymentMethod paymentMethod)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var payment = await _context.PaymentMethods.FirstOrDefaultAsync(x => x.Name == paymentMethod.Name);
                if (payment != null)
                {
                    return null;
                }
                _context.PaymentMethods.Add(paymentMethod);
                await _context.SaveChangesAsync();
                transaction.Commit();
                return paymentMethod;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return null;
            }
        }
        public async Task<PaymentMethod> updatePaymentMethod(int id, string updateName, string updateDescription)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var payment = await _context.PaymentMethods.FindAsync(id);
                if (payment == null)
                {
                    return null;
                }
                if (updateName != null)
                {
                    payment.Name = updateName;
                }
                if (updateDescription != null)
                {
                    payment.Description = updateDescription;
                }
                await _context.SaveChangesAsync();
                transaction.Commit();
                return payment;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return null;
            }
        }
    }
}