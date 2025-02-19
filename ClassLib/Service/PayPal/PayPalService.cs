// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net;
// using System.Threading.Tasks;
// using ClassLib.DTO.Payment;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Options;
// using Microsoft.VisualBasic;

// namespace ClassLib.Service.PayPal
// {
//     public class PayPalService : IPayPalService
//     {
//         private readonly IOptions<PaypalOptionModel> _options;
//         private const double ExchangeRate = 25_520.0;
//         public PayPalService(IOptions<PaypalOptionModel> options)
//         {
//             _options = options;
//         }
//         public static double ConvertVndToDollar(double vnd)
//         {
//             var total = Math.Round(vnd / ExchangeRate, 2);

//             return total;
//         }

//         public async Task<string> CreatePaymentUrl(PaymentInformationModel model)
//         {
//             var env = new SandboxEnvironment(_options.Value.ClientId, _options.Value.SecretKey);
//             var client = new PayPalHttpClient(env);
//             var paypalOrderID = DateTime.UtcNow.Ticks.ToString();
//             var urlCallBack = _options.Value.ReturnUrl;
//             var payment = new PayPal.v1.Payments.Payment()
//             {
//                 Intent = "sale",
//                 Transactions = new List<Transaction>()
//                 {
//                     new Transaction()
//                     {
//                     Amount = new Amount()
//                     {
//                         Total = ConvertVndToDollar(model.Amount).ToString(),
//                         Currency = "USD",
//                         Details = new AmountDetails
//                         {
//                             Tax = "0",
//                             Shipping = "0",
//                             Subtotal = ConvertVndToDollar(model.Amount).ToString(),
//                         }
//                     },
//                     ItemList = new ItemList()
//                     {
//                         Items = new List<Item>()
//                         {
//                             new Item()
//                             {
//                                 Name = " | Order: " + model.OrderDescription,
//                                 Currency = "USD",
//                                 Price = ConvertVndToDollar(model.Amount).ToString(),
//                                 Quantity = "1",
//                                 Sku = "sku",
//                                 Tax = "0",
//                                 Url = "https://www.code-mega.com" // Url detail of Item
//                             }
//                         }
//                     },
//                     Description = $"Invoice #{model.OrderDescription}",
//                     InvoiceNumber = paypalOrderID.ToString()
//                     }
//                 },
//                 RedirectUrls = new RedirectUrls()
//                 {
//                     ReturnUrl = $"{urlCallBack}?payment_method=PayPal&success=1&order_id={paypalOrderId}",
//                     CancelUrl = $"{urlCallBack}?payment_method=PayPal&success=0&order_id={paypalOrderId}"
//                 },
//                 Payer = new Payer()
//                 {
//                     PaymentMethod = "paypal"
//                 }
//             };

//             var request = new PaymentCreateRequest();
//             request.RequestBody(payment);

//             var paymentUrl = "";
//             var response = await client.Execute(request);
//             var statusCode = response.StatusCode;

//             if (statusCode is not (HttpStatusCode.Accepted or HttpStatusCode.OK or HttpStatusCode.Created))
//             {
//                 return paymentUrl;
//             }

//             var result = response.Result<Payment>();
//             using var links = result.Links.GetEnumerator();

//             while (links.MoveNext())
//             {
//                 var lnk = links.Current;
//                 if (lnk == null) continue;

//                 if (!lnk.Rel.ToLower().Trim().Equals("approval_url")) continue;

//                 paymentUrl = lnk.Href;
//             }

//             return paymentUrl;
//         }

//         public PaymentResponseModel PaymentExecute(IQueryCollection collections)
//         {
//             throw new NotImplementedException();
//         }
//     }
//     {
        
//     }
// }