using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClassLib.DTO.Payment;
using ClassLib.Enum;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using TimeProvider = ClassLib.Helpers.TimeProvider;

namespace ClassLib.Service.PaymentService
{
    public class MomoServices : IPaymentServices
    {
        private readonly IOptions<MomoConfigFromJSON> _momoConfig;

        private readonly PaymentRepository _paymentRepository;

        private readonly PaymentMethodRepository _paymentMethodRepository;

        public MomoServices(IOptions<MomoConfigFromJSON> momoConfig, PaymentRepository paymentRepository, PaymentMethodRepository paymentMethodRepository)
        {
            _momoConfig = momoConfig;
            _paymentRepository = paymentRepository;
            _paymentMethodRepository = paymentMethodRepository;
        }
        public async Task<string> CreatePaymentURL(OrderInfoModel orderInfo, HttpContext context)
        {
            orderInfo.OrderId = TimeProvider.GetVietnamNow().Ticks.ToString();
            var rawData =
                $"partnerCode={_momoConfig.Value.PartnerCode}&accessKey={_momoConfig.Value.AccessKey}&requestId={orderInfo.OrderId}&amount={((long)Math.Floor(orderInfo.Amount)).ToString()}&orderId={orderInfo.OrderId}&orderInfo={orderInfo.OrderDescription + " " + orderInfo.GuestName + " " + orderInfo.GuestEmail}&returnUrl={_momoConfig.Value.ReturnUrl}&notifyUrl={_momoConfig.Value.NotifyUrl}&extraData={orderInfo.BookingID}";
            var signature = ComputeHmacSha256(rawData, _momoConfig.Value.SecretKey);

            var client = new RestClient(_momoConfig.Value.MomoApiUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            var requestData = new
            {
                accessKey = _momoConfig.Value.AccessKey,
                partnerCode = _momoConfig.Value.PartnerCode,
                requestType = _momoConfig.Value.RequestType,
                notifyUrl = _momoConfig.Value.NotifyUrl,
                returnUrl = _momoConfig.Value.ReturnUrl,
                orderId = orderInfo.OrderId,
                amount = ((long)Math.Floor(orderInfo.Amount)).ToString(),
                orderInfo = orderInfo.OrderDescription + " " + orderInfo.GuestName + " " + orderInfo.GuestEmail,
                requestId = orderInfo.OrderId,
                extraData = orderInfo.BookingID,
                signature = signature
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);

            string jsonString = response.Content!;
            JObject json = JObject.Parse(jsonString);
            string payUrl = json["payUrl"]?.ToString()!;
            return payUrl!;
        }

        public async Task<string> CreateRefund(RefundModel refundModel, HttpContext context)
        {
            var orderId = TimeProvider.GetVietnamNow().Ticks.ToString();
            var partnerCode = _momoConfig.Value.PartnerCode;

            // Ensure all required fields are present and in the correct order (without requestType)
            var rawSignature = $"accessKey={_momoConfig.Value.AccessKey}"
                             + $"&amount={((long)Math.Floor(refundModel.amount))}"
                             + $"&description="
                             + $"&orderId={orderId}"
                             + $"&partnerCode={partnerCode}"
                             + $"&requestId={orderId}"
                             + $"&transId={refundModel.trancasionID}";

            var signature = ComputeHmacSha256(rawSignature, _momoConfig.Value.SecretKey);

            var requestData = new
            {
                accessKey = _momoConfig.Value.AccessKey,
                partnerCode = partnerCode,
                orderId = orderId,
                amount = ((long)Math.Floor(refundModel.amount)).ToString(),
                requestId = orderId,
                transId = refundModel.trancasionID,
                description = "",
                lang = "vi",
                signature = signature
            };

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://test-payment.momo.vn/v2/gateway/api/refund", jsonContent);

            return await response.Content.ReadAsStringAsync();
        }



        public async Task<RespondModel> GetPaymentStatus(IQueryCollection collection)
        {
            var amount = collection.FirstOrDefault(s => s.Key == "amount").Value;
            var orderInfo = collection.FirstOrDefault(s => s.Key == "orderInfo").Value;
            var orderId = collection.FirstOrDefault(s => s.Key == "orderId").Value;
            var message = (collection.FirstOrDefault(s => s.Key == "message").Value == "Success") ? "Success" : "Failed";
            var trancasionID = collection.FirstOrDefault(s => s.Key == "transId").Value;
            var bookingID = collection.FirstOrDefault(s => s.Key == "extraData").Value;
            Payment payment = new Payment()
            {
                PaymentId = orderId!,
                PayerId = orderInfo.ToString().Split(" ")[1],
                TransactionId = trancasionID!,
                Currency = "VND",
                PaymentDate = TimeProvider.GetVietnamNow(),
                TotalPrice = decimal.Parse(amount!),
                PaymentMethod = (await _paymentMethodRepository.getPaymentMethodByName("momo"))!.Id,
                Status = message,
                BookingId = int.Parse(bookingID!)
            };
            await _paymentRepository.AddPayment(payment);

            return await Task.FromResult(new RespondModel()
            {
                Amount = amount!,
                OrderId = orderId!,
                OrderDescription = orderInfo!,
                Message = message!,
                TrancasionID = trancasionID!,
                BookingID = bookingID!
            });
        }

        public string PaymentName()
        {
            return "momo";
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }
    }
}