using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PaymentAPI.Model;
using PayPal.Core;
using PayPal.v1.Payments;

namespace PaymentAPI.Services
{
    public class PaypalServices : IPaymentServices
    {
        private readonly IOptions<PaypalConfigFromJson> _paypalConfig;
        private const double ExchangeRate = 25535.00;

        public PaypalServices(IOptions<PaypalConfigFromJson> paypalConfig)
        {
            _paypalConfig = paypalConfig;
        }
        public static double ConvertVndToDollar(double vnd)
        {
            var total = Math.Round(vnd / ExchangeRate, 2);

            return total;
        }
        public async Task<string> CreatePaymentURL(OrderInfoModel orderInfo, HttpContext context)
        {
            var envSandbox = new SandboxEnvironment(_paypalConfig.Value.ClientId, _paypalConfig.Value.SecretKey);
            var client = new PayPalHttpClient(envSandbox);
            var paypalOrderId = DateTime.Now.Ticks;
            var urlCallBack = _paypalConfig.Value.ReturnUrl;

            var payment = new PayPal.v1.Payments.Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Amount()
                        {
                            Total = ConvertVndToDollar((double)orderInfo.Amount).ToString(),
                            Currency = "USD",
                            Details = new AmountDetails
                            {
                                Tax = "0",
                                Shipping = "0",
                                Subtotal = ConvertVndToDollar((double)orderInfo.Amount).ToString(),
                            }
                        },
                        Description = $"Invoice #{orderInfo.OrderDescription}",
                        InvoiceNumber = paypalOrderId.ToString()
                    }
                },
                RedirectUrls = new RedirectUrls()
                {
                    ReturnUrl =
                        $"{urlCallBack}?payment_method=PayPal&success=1&order_id={paypalOrderId}&amount={ConvertVndToDollar((double)orderInfo.Amount).ToString()}&order_description={orderInfo.OrderDescription}",
                    CancelUrl =
                        $"{urlCallBack}?payment_method=PayPal&success=0&order_id={paypalOrderId}"
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };

            var request = new PaymentCreateRequest();
            request.RequestBody(payment);

            var paymentUrl = "";
            var response = await client.Execute(request);
            var statusCode = response.StatusCode;

            if (statusCode is not (HttpStatusCode.Accepted or HttpStatusCode.OK or HttpStatusCode.Created))
                return paymentUrl;

            var result = response.Result<Payment>();
            using var links = result.Links.GetEnumerator();


            while (links.MoveNext())
            {
                var lnk = links.Current;
                if (lnk == null) continue;
                if (!lnk.Rel.ToLower().Trim().Equals("approval_url")) continue;
                paymentUrl = lnk.Href;
            }

            return paymentUrl;
        }

        public async Task<RespondModel> GetPaymentStatus(IQueryCollection collection)
        {
            var amount = collection.FirstOrDefault(s => s.Key == "amount").Value;
            var orderInfo = collection.FirstOrDefault(s => s.Key == "order_description").Value;
            var orderId = collection.FirstOrDefault(s => s.Key == "order_id").Value;
            var message = collection.FirstOrDefault(s => s.Key == "success").Value;
            var trancasionID = collection.FirstOrDefault(s => s.Key == "paymentId").Value;
            var BookingID = collection.FirstOrDefault(s => s.Key == "BookingID").Value;
            return await Task.FromResult(new RespondModel()
            {
                Amount = amount!,
                OrderId = orderId!,
                OrderDescription = orderInfo!,
                Message = message!,
                TrancasionID = trancasionID!,
                BookingID = BookingID!
            });
        }

        public string PaymentName()
        {
            return "paypal";
        }
    }
}