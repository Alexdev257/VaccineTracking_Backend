using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using ClassLib.DTO.Payment;
using ClassLib.Enum;
using Microsoft.AspNetCore.Http;

namespace ClassLib.Service.PaymentService
{
    public class CashServices : IPaymentServices
    {
        public Task<string> CreatePaymentURL(OrderInfoModel orderInfo, HttpContext context)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateRefund(RefundModel refundModel, HttpContext context)
        {
            throw new NotImplementedException();
        }

        public Task<RespondModel> GetPaymentStatus(IQueryCollection collection)
        {
            throw new NotImplementedException();
        }

        public string PaymentName() => PaymentEnum.Cash.ToString();
    }
}