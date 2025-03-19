using ClassLib.DTO.Payment;
using ClassLib.Enum;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using TimeProvider = ClassLib.Helpers.TimeProvider;

namespace ClassLib.Service.PaymentService
{
    public class CashServices : IPaymentServices
    {
                private readonly PaymentRepository _paymentRepository;

        private readonly PaymentMethodRepository _paymentMethodRepository;

        public CashServices( PaymentRepository paymentRepository, PaymentMethodRepository paymentMethodRepository){
            _paymentMethodRepository = paymentMethodRepository;
            _paymentRepository = paymentRepository;
        }
        public Task<string> CreatePaymentURL(OrderInfoModel orderInfo, HttpContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CreateRefund(RefundModel refundModel, HttpContext context)
        {
            // public double amount { get; set; } = 0;
            // public string paymentID { get; set; } = string.Empty;
            // public string payerID { get; set; } = string.Empty;
            // public DateTime paymentDate { get; set; } = DateTime.Now;
            // public string currency { get; set; } = string.Empty;
            // public string trancasionID { get; set; } = string.Empty;
            // public int RefundType { get; set; } = 0;
            Payment payment = new Payment(){
                PayerId = refundModel.payerID,
                TotalPrice = (decimal)refundModel.amount * -1,
                PaymentId = TimeProvider.GetVietnamNow().Ticks.ToString(),
                PaymentDate = TimeProvider.GetVietnamNow(),
                TransactionId = TimeProvider.GetVietnamNow().Ticks.ToString(),
                Currency = refundModel.currency,
                PaymentMethod = (await _paymentMethodRepository.getPaymentMethodByName(PaymentEnum.Cash.ToString()))!.Id,
                Status = ((PaymentStatusEnum)refundModel.RefundType).ToString(),
                BookingId = (await _paymentRepository.GetByIDAsync(refundModel.paymentID))!.BookingId,
            };
            await _paymentRepository.AddPayment(payment);

            return PaymentStatusEnum.Success.ToString();
        }

        public Task<RespondModel> GetPaymentStatus(IQueryCollection collection)
        {
            throw new NotImplementedException();
        }

        public string PaymentName() => PaymentEnum.Cash.ToString();
    }
}