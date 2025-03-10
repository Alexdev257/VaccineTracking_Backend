using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ClassLib.DTO.Payment;
using ClassLib.Enum;
using ClassLib.Helpers;
using ClassLib.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PayPal.Core;
using PayPal.v1.Payments;
using ModelPayment = ClassLib.Models.Payment;
using TimeProvider = ClassLib.Helpers.TimeProvider;

namespace ClassLib.Service.PaymentService
{
    public class PaypalServices : IPaymentServices
    {
        private readonly IOptions<PaypalConfigFromJson> _paypalConfig;
        private const double ExchangeRate = 25535.00;

        private readonly PaymentRepository _paymentRepository;

        private readonly PaymentMethodRepository _paymentMethodRepository;

        private readonly BookingRepository _bookingRepository;

        public PaypalServices(IOptions<PaypalConfigFromJson> paypalConfig
                            , PaymentRepository paymentRepository
                            , PaymentMethodRepository paymentMethodRepository
                            , BookingRepository bookingRepository)
        {
            _paypalConfig = paypalConfig;
            _paymentRepository = paymentRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _bookingRepository = bookingRepository;
        }
        public static double ConvertVndToDollar(double vnd)
        {
            var total = Math.Round(vnd / ExchangeRate, 2);

            return total;
        }

        public static double ConvertDollarToVnd(double dollar)
        {
            var total = Math.Round(dollar * ExchangeRate, 2);

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
                        $"{urlCallBack}?payment_method=PayPal&success=1&order_id={paypalOrderId}&amount={ConvertVndToDollar((double)orderInfo.Amount).ToString()}&order_description={orderInfo.OrderDescription}&BookingID={orderInfo.BookingID}",
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
            var totalPrice = collection.FirstOrDefault(s => s.Key == "amount").Value;
            var paymentId = collection.FirstOrDefault(s => s.Key == "paymentId").Value;
            var status = (collection.FirstOrDefault(s => s.Key == "success").Value == "1") ? "Success" : "Fail";
            var BookingID = collection.FirstOrDefault(s => s.Key == "BookingID").Value;
            var PaymentDate = DateTime.Now;
            var currency = "USD";
            var paymentMethod = (await _paymentMethodRepository.getPaymentMethodByName("paypal"))!.Id;
            var payerId = collection.FirstOrDefault(s => s.Key == "PayerID").Value;
            var TrancasionID = await GetSaleID(paymentId!, payerId!);


            ModelPayment payment = new()
            {
                PaymentId = paymentId!,
                BookingId = int.Parse(BookingID!),
                TransactionId = TrancasionID,
                PayerId = payerId!,
                PaymentMethod = paymentMethod,
                Currency = currency,
                TotalPrice = decimal.Parse(totalPrice!),
                PaymentDate = PaymentDate,
                Status = status,
                IsDeleted = false
            };
            await _paymentRepository.AddPayment(payment);

            return await Task.FromResult(new RespondModel()
            {
                Amount = totalPrice!,
                OrderId = paymentId!,
                OrderDescription = collection.FirstOrDefault(s => s.Key == "order_description").Value!,
                Message = status,
                TrancasionID = TrancasionID!,
                BookingID = BookingID!
            });
        }

        public async Task<string> GetAccessTokenAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_paypalConfig.Value.ClientId}:{_paypalConfig.Value.SecretKey}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                var requestData = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.PostAsync($"{_paypalConfig.Value.BaseUrl}/v1/oauth2/token", requestData);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error getting access token: {response.StatusCode}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseBody);
                string accessToken = doc.RootElement.GetProperty("access_token").GetString()!;
                return accessToken;
            }
        }

        public async Task<string> GetSaleID(string paymentID, string payerID)
        {
            using (HttpClient client = new HttpClient())
            {
                var authToken = await GetAccessTokenAsync();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string executeUrl = $"https://api.sandbox.paypal.com/v1/payments/payment/{paymentID}/execute";

                var executeReq = new
                {
                    payer_id = payerID
                };

                var content = new StringContent(JsonSerializer.Serialize(executeReq), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(executeUrl, content);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Payment execution failed: {response.StatusCode} - {jsonResponse}");
                }
                string sale = ExtractSaleId(jsonResponse);
                return sale;
            }
            ;
        }

        public string ExtractSaleId(string jsonResponse)
        {
            using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
            {
                var root = doc.RootElement;

                // Navigate to "transactions" -> "related_resources" -> "sale" -> "id"
                var saleId = root
                    .GetProperty("transactions")[0]
                    .GetProperty("related_resources")[0]
                    .GetProperty("sale")
                    .GetProperty("id")
                    .GetString();

                return saleId ?? throw new Exception("Sale ID not found in response.");
            }
        }

        public string PaymentName()
        {
            return "paypal";
        }

        public async Task<string> CreateRefund(RefundModel refundModel, HttpContext context)
        {
            using (HttpClient client = new HttpClient())
            {
                var authToken = await GetAccessTokenAsync();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string refundUrl = $"https://api.sandbox.paypal.com/v1/payments/sale/{refundModel.trancasionID}/refund";

                var refundRequest = new
                {
                    amount = new
                    {
                        total = refundModel.amount.ToString("F2", CultureInfo.InvariantCulture),
                        currency = refundModel.currency
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(refundRequest), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(refundUrl, content);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Refund failed: {response.StatusCode} - {jsonResponse}");
                }
                using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                string refundId = doc.RootElement.GetProperty("id").GetString()!;
                string trancasionID = doc.RootElement.GetProperty("parent_payment").GetString()!;
                string message = doc.RootElement.GetProperty("state").GetString()!;

                if (message == "completed")
                {
                    ModelPayment payment = new()
                    {
                        PaymentId = refundId,
                        BookingId = (await _paymentRepository.GetByIDAsync(refundModel.paymentID))!.BookingId,
                        PayerId = (await _bookingRepository.GetByBookingID((await _paymentRepository.GetByIDAsync(refundModel.paymentID))!.BookingId))!.ParentId.ToString(),
                        PaymentMethod = (await _paymentMethodRepository.getPaymentMethodByName("paypal"))!.Id,
                        Currency = "USD",
                        TransactionId = trancasionID,
                        TotalPrice = ((decimal)refundModel.amount) * -1,
                        PaymentDate = TimeProvider.GetVietnamNow(),
                        Status = ((PaymentStatusEnum)refundModel.RefundType).ToString(),
                        IsDeleted = false
                    };
                    await _paymentRepository.AddPayment(payment);

                    //return message;

                    return "Success";
                }

                return "This booking cannot return please go to apointment to have better services";
            }
        }
    }
}