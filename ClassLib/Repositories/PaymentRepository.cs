using ClassLib.Enum;
using ClassLib.Models;
using ClassLib.Service;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class PaymentRepository
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;

        private readonly PaymentMethodRepository _paymentMethodRepository;
        public PaymentRepository(DbSwpVaccineTrackingFinalContext context, PaymentMethodRepository paymentMethodRepository)
        {
            _context = context;
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<List<Payment>> GetAllAsync() => await _context.Payments.ToListAsync();

        public async Task<Payment?> GetByIDAsync(string id) => await _context.Payments.FindAsync(id);

        public async Task<Payment?> GetByBookingIDAsync(int id) => await _context.Payments.Where(p => p.BookingId == id).OrderByDescending(p => p.PaymentDate).FirstOrDefaultAsync();

        public async Task<string> GetPaymentNameOfBooking(string id)
        {
            Payment payment = (await GetByBookingIDAsync(int.Parse(id)))!;
            return (await _paymentMethodRepository.getPaymentMethodById(payment.PaymentMethod))!.Name;
        }

        public async Task<Payment> UpdateStatusPayment(string id, string msg)
        {
            Payment payment = (await GetByIDAsync(id))!;
            payment.Status = msg;
            await _context.SaveChangesAsync();
            return payment;
        }
        public async Task<Payment> AddPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}