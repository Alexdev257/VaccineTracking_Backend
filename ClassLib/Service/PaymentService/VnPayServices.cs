using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PaymentAPI.Model;

namespace PaymentAPI.Services
{
    public class VnPayServices : IPaymentServices
    {
        private readonly IOptions<VnPayConfigFromJson> _vnpayConfig;
        private readonly string TimeZoneID = "SE Asia Standard Time";

        public VnPayServices(IOptions<VnPayConfigFromJson> vnpayConfig)
        {
            _vnpayConfig = vnpayConfig;
        }
        public Task<string> CreatePaymentURL(OrderInfoModel orderInfo, HttpContext context)
        {
            var timeZoneID = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneID);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();

            pay.AddRequestData("vnp_Version", _vnpayConfig.Value.Version);
            pay.AddRequestData("vnp_Command", _vnpayConfig.Value.Command);
            pay.AddRequestData("vnp_TmnCode", _vnpayConfig.Value.TmnCode);
            pay.AddRequestData("vnp_Amount", ((int)orderInfo.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _vnpayConfig.Value.CurrCode);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _vnpayConfig.Value.Locale);
            pay.AddRequestData("vnp_OrderInfo", $"{orderInfo.GuestName} {orderInfo.OrderDescription} {orderInfo.Amount}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", _vnpayConfig.Value.ReturnUrl);
            pay.AddRequestData("vnp_TxnRef", tick);
            pay.AddRequestData("vnp_ExpireDate", timeNow.AddMinutes(5).ToString("yyyyMMddHHmmss"));
            
            return Task.FromResult(pay.CreateRequestUrl(_vnpayConfig.Value.BaseUrl, _vnpayConfig.Value.HashSecret));
        }

        public async Task<RespondModel> GetPaymentStatus(IQueryCollection collection)
        {
            var amount = collection.FirstOrDefault(s => s.Key == "vnp_Amount").Value;
            var orderInfo = collection.FirstOrDefault(s => s.Key == "vnp_OrderInfo").Value;
            var orderId = collection.FirstOrDefault(s => s.Key == "vnp_TxnRef").Value;
            var message = collection.FirstOrDefault(s => s.Key == "vnp_ResponseCode").Value;
            var trancasionID = collection.FirstOrDefault(s => s.Key == "vnp_TransactionNo").Value;
            return await Task.FromResult(new RespondModel()
            {
                Amount = amount!,
                OrderId = orderId!,
                OrderDescription = orderInfo!,
                Message = message!,
                TrancasionID = trancasionID!
            });
        }

        public string PaymentName()
        {
            return "vnpay";
        }
    }
}