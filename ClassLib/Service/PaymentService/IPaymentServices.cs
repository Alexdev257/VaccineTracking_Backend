using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.Payment;
using Microsoft.AspNetCore.Http;

namespace ClassLib.Service.PaymentService
{
    public interface IPaymentServices
    {
        string PaymentName();
        Task<string> CreatePaymentURL(OrderInfoModel orderInfo, HttpContext context);
        Task<RespondModel> GetPaymentStatus(IQueryCollection collection);
        Task<string> CreateRefund(RefundModel refundModel, HttpContext context);
    }
}