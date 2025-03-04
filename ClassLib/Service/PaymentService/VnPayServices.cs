using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ClassLib.DTO.Payment;
using ClassLib.Enum;
using ClassLib.Models;
using ClassLib.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ClassLib.Service.PaymentService
{
    public class VnPayServices : IPaymentServices
    {
        private readonly IOptions<VnPayConfigFromJson> _vnpayConfig;
        private readonly string TimeZoneID = "SE Asia Standard Time";

        private readonly PaymentRepository _paymentRepository;

        private readonly PaymentMethodRepository _paymentMethodRepository;

        public VnPayServices(IOptions<VnPayConfigFromJson> vnpayConfig, PaymentRepository paymentRepository, PaymentMethodRepository paymentMethodRepository)
        {
            _vnpayConfig = vnpayConfig;
            _paymentRepository = paymentRepository;
            _paymentMethodRepository = paymentMethodRepository;
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
            pay.AddRequestData("vnp_OrderInfo", $"{orderInfo.GuestName} {orderInfo.OrderDescription} {orderInfo.Amount} {orderInfo.GuestPhone} {orderInfo.GuestEmail} bookingID{orderInfo.BookingID}");
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
            var message = (collection.FirstOrDefault(s => s.Key == "vnp_ResponseCode").Value == "00") ? "Success" : "Failed";
            var trancasionID = collection.FirstOrDefault(s => s.Key == "vnp_TransactionNo").Value;
            var booking = orderInfo.ToString().Split("bookingID")[1];
            var paymentdate = collection.FirstOrDefault(s => s.Key == "vnp_PayDate").Value;
            var payerID = orderInfo.ToString().Split(" ")[0];
            Payment payment = new Payment()
            {
                PaymentId = orderId!,
                PaymentMethod = (await _paymentMethodRepository.getPaymentMethodByName("vnpay")).Id,
                TotalPrice = int.Parse(amount!),
                BookingId = int.Parse(booking),
                PaymentDate = DateTime.ParseExact(paymentdate, "yyyyMMddHHmmss", null),
                Status = message,
                TransactionId = int.Parse(trancasionID),
                PayerId = int.Parse(payerID)

            };
            await _paymentRepository.AddPayment(payment);

            return await Task.FromResult(new RespondModel()
            {
                Amount = amount!,
                OrderId = orderId!,
                OrderDescription = orderInfo!,
                Message = message,
                TrancasionID = trancasionID!,
                BookingID = booking
            });
        }

        public string PaymentName()
        {
            return "vnpay";
        }
    }
}