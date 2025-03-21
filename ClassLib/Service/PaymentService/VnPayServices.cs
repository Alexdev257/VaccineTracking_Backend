using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ClassLib.DTO.Payment;
using ClassLib.Enum;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TimeProvider = ClassLib.Helpers.TimeProvider;

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

        public string HmacSha512ForRefund(string key, string inputData)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);

            using HMACSHA512 hmac = new HMACSHA512(keyBytes);
            byte[] hashBytes = hmac.ComputeHash(inputBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public async Task<string> CreateRefund(RefundModel refundModel, HttpContext context)
        {
            return await Task.FromResult("Go to appointment to have better services");
            // var timeZoneID = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID);
            // var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneID);
            // var tick = DateTime.Now.Ticks.ToString();
            // var pay = new VnPayLibrary();
            // // pay.AddRequestData("vnp_RequestId", tick);
            // // pay.AddRequestData("vnp_Version", _vnpayConfig.Value.Version);
            // // pay.AddRequestData("vnp_Command", "refund");
            // // pay.AddRequestData("vnp_TmnCode", _vnpayConfig.Value.TmnCode);
            // // pay.AddRequestData("vnp_TransactionType", "02");
            // // pay.AddRequestData("vnp_TxnRef", refundModel.trancasionID);
            // // pay.AddRequestData("vnp_Amount", ((int)refundModel.amount * 100).ToString());
            // // pay.AddRequestData("vnp_OrderInfo", $"hi");
            // // pay.AddRequestData("vnp_TransactionDate", refundModel.paymentDate.ToString("yyyyMMddHHmmss"));
            // // pay.AddRequestData("vnp_CreateBy", "NGUYEN VAN A");
            // // pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            // // pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));

            // using HttpClient client = new HttpClient();
            // string url = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";
            // // Step 1: Prepare the data string in the exact format
            // string data = string.Join("|", new string[]
            // {
            //     TimeProvider.GetVietnamNow().Ticks.ToString(),  // vnp_RequestId
            //     _vnpayConfig.Value.Version,                    // vnp_Version
            //     "refund",                                      // vnp_Command
            //     _vnpayConfig.Value.TmnCode,                    // vnp_TmnCode
            //     "3",                                          // vnp_TransactionType
            //     refundModel.trancasionID,                      // vnp_TxnRef
            //     refundModel.amount.ToString(),                 // vnp_Amount
            //     refundModel.trancasionID,                     // vnp_TransactionNo
            //     refundModel.paymentDate.ToString("yyyyMMddHHmmss"), // vnp_TransactionDate
            //     "NGUYEN VAN A",                                // vnp_CreateBy
            //     TimeProvider.GetVietnamNow().ToString("yyyyMMddHHmmss"), // vnp_CreateDate
            //     pay.GetIpAddress(context),                     // vnp_IpAddr
            //     "hi"                                           // vnp_OrderInfo
            // });

            // // Step 2: Generate the Secure Hash (checksum)
            // string secretKey = _vnpayConfig.Value.HashSecret;
            // string checksum = HmacSha512ForRefund(secretKey, data);

            // // Step 3: Add checksum to the request
            // var requestData = new
            // {
            //     vnp_RequestId = TimeProvider.GetVietnamNow().Ticks.ToString(),
            //     vnp_Version = _vnpayConfig.Value.Version,
            //     vnp_Command = "refund",
            //     vnp_TmnCode = _vnpayConfig.Value.TmnCode,
            //     vnp_TransactionType = "3",
            //     vnp_TxnRef = refundModel.trancasionID,
            //     vnp_Amount = refundModel.amount,
            //     vnp_TransactionNo = refundModel.trancasionID,
            //     vnp_TransactionDate = refundModel.paymentDate.ToString("yyyyMMddHHmmss"),
            //     vnp_CreateBy = "NGUYEN VAN A",
            //     vnp_CreateDate = TimeProvider.GetVietnamNow().ToString("yyyyMMddHHmmss"),
            //     vnp_IpAddr = pay.GetIpAddress(context),
            //     vnp_OrderInfo = "hi",
            //     vnp_SecureHash = checksum  // Add the secure hash here
            // };



            // string json = JsonSerializer.Serialize(requestData);
            // var content = new StringContent(json, Encoding.UTF8, "application/json");

            // HttpResponseMessage response = await client.PostAsync(url, content);
            // string responseBody = await response.Content.ReadAsStringAsync();
            // Console.WriteLine($"Status Code: {response.StatusCode}");
            // Console.WriteLine($"Response Body: {responseBody}");
            // Console.WriteLine("Raw Data for Hashing: " + data);
            // System.Console.WriteLine("After Hashing" + checksum);
            // if (response.IsSuccessStatusCode)
            // {
            //     string result = await response.Content.ReadAsStringAsync();
            //     Console.WriteLine("Response: " + result);
            // }
            // else
            // {
            //     Console.WriteLine("Error: " + response.StatusCode);
            // }

            // return await response.Content.ReadAsStringAsync();
            //return await Task.FromResult(pay.CreateRequestUrl("https://sandbox.vnpayment.vn/merchant_webapi/api/transaction", _vnpayConfig.Value.HashSecret));

        }

        public async Task<RespondModel> GetPaymentStatus(IQueryCollection collection)
        {
            var amount = decimal.Parse(collection.FirstOrDefault(s => s.Key == "vnp_Amount").Value!) / 100;
            var orderInfo = collection.FirstOrDefault(s => s.Key == "vnp_OrderInfo").Value;
            var orderId = collection.FirstOrDefault(s => s.Key == "vnp_TransactionNo").Value;
            var message = (collection.FirstOrDefault(s => s.Key == "vnp_ResponseCode").Value == "00") ? PaymentStatusEnum.Success.ToString() : PaymentStatusEnum.Failed.ToString();
            var trancasionID = collection.FirstOrDefault(s => s.Key == "vnp_TxnRef").Value;
            var bookingId = orderInfo.ToString().Split("bookingID")[1];
            System.Console.WriteLine(bookingId);
            var paymentdate = collection.FirstOrDefault(s => s.Key == "vnp_PayDate").Value;
            var payerID = orderInfo.ToString().Split(" ")[0];
            var currency = "VND";

            if (message == PaymentStatusEnum.Success.ToString())
            {
                Payment payment = new Payment()
                {
                    PaymentId = orderId!,
                    BookingId = int.Parse(bookingId),
                    TransactionId = trancasionID!,
                    PayerId = payerID,
                    PaymentMethod = (await _paymentMethodRepository.getPaymentMethodByName(PaymentEnum.VnPay.ToString()))!.Id,
                    Currency = currency,
                    TotalPrice = amount,
                    PaymentDate = DateTime.ParseExact(paymentdate!, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture),
                    Status = message,
                    IsDeleted = false
                };

                await _paymentRepository.AddPayment(payment);
            }

            return await Task.FromResult(new RespondModel()
            {
                Amount = amount.ToString(),
                OrderId = orderId!,
                OrderDescription = orderInfo!,
                Message = message,
                TrancasionID = trancasionID!,
                BookingID = bookingId
            });
        }

        public string PaymentName() => PaymentEnum.VnPay.ToString();
    }
}