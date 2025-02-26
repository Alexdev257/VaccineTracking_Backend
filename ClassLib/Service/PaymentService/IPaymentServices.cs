using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PaymentAPI.Model;

namespace PaymentAPI.Services
{
    public interface IPaymentServices
    {
        string PaymentName();
        Task<string> CreatePaymentURL(OrderInfoModel orderInfo, HttpContext context);
        Task<RespondModel> GetPaymentStatus(IQueryCollection collection);
    }
}