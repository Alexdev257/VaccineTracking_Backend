using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentAPI.Model
{
    public class PaypalConfigFromJson
    {
        public string ClientId { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
    }
}