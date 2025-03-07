using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.Enum;
using ClassLib.Models;
using ClassLib.Service;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class PaymentRepository
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;

        private readonly BookingRepository _bookingRepository;

        private readonly PaymentMethodRepository _paymentMethodRepository;

        private readonly VaccinesTrackingService _vaccinesTrackingService;
        public PaymentRepository(DbSwpVaccineTrackingFinalContext context, BookingRepository bookingRepository, PaymentMethodRepository paymentMethodRepository, VaccinesTrackingService vaccinesTrackingService)
        {
            _context = context;
            _bookingRepository = bookingRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _vaccinesTrackingService = vaccinesTrackingService;
        }

        public async Task<List<Payment>> GetAllAsync() => await _context.Payments.ToListAsync();

        public async Task<Payment?> GetByIDAsync(string id) => await _context.Payments.FindAsync(id);

        public async Task<Payment?> GetByBookingIDAsync(int id) => await _context.Payments.Where(p => p.BookingId == id).OrderByDescending(p => p.PaymentDate).FirstOrDefaultAsync();

        public async Task<string> GetPaymentNameOfBooking(string id)
        {
            Payment payment = (await GetByBookingIDAsync(int.Parse(id)))!;
            return (await _paymentMethodRepository.getPaymentMethodById(payment.PaymentMethod))!.Name;
        }

        public async Task<bool> UpdateStatusPayment(string id, string msg)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Payment payment = (await GetByIDAsync(id))!;
                payment.Status = msg;
                await _bookingRepository.UpdateBooking((payment.BookingId).ToString(), ((BookingEnum)BookingEnum.Refund).ToString());
                await _vaccinesTrackingService.VaccinesTrackingRefund(payment.BookingId, VaccinesTrackingEnum.Cancel);
                var entry = _context.Entry(payment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                await transaction.RollbackAsync();
                return false;
            }
        }
        public async Task<Payment> AddPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}