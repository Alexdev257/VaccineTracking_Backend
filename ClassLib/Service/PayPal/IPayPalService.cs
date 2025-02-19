using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.Payment;
using Microsoft.AspNetCore.Http;

namespace ClassLib.Service.PayPal
{
    public interface IPayPalService
    {
        Task<string> CreatePaymentUrl(PaymentInformationModel model);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}