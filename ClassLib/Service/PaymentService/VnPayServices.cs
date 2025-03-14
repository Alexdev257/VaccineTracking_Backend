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

        public async Task<string> CreateRefund(RefundModel refundModel, HttpContext context)
        {
            // var timeZoneID = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID);
            // var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneID);
            // var tick = DateTime.Now.Ticks.ToString();
            // var pay = new VnPayLibrary();
            // pay.AddRequestData("vnp_RequestId", tick);
            // pay.AddRequestData("vnp_Version", _vnpayConfig.Value.Version);
            // pay.AddRequestData("vnp_Command", "refund");
            // pay.AddRequestData("vnp_TmnCode", _vnpayConfig.Value.TmnCode);
            // pay.AddRequestData("vnp_TransactionType", "02");
            // pay.AddRequestData("vnp_TxnRef", refundModel.trancasionID);
            // pay.AddRequestData("vnp_Amount", ((int)refundModel.amount * 100).ToString());
            // pay.AddRequestData("vnp_OrderInfo", $"hi");
            // pay.AddRequestData("vnp_TransactionDate", refundModel.paymentDate.ToString("yyyyMMddHHmmss"));
            // pay.AddRequestData("vnp_CreateBy", "NGUYEN VAN A");
            // pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            // pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));

            // return Task.FromResult(pay.CreateRequestUrl(_vnpayConfig.Value.BaseUrl, _vnpayConfig.Value.HashSecret));

            return "This booking cannot return please go to apointment to have better services";
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