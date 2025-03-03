using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.PaymentMethod
{
    public class AddPaymentMethod
    {
        public string Name { get; set; } = string.Empty;

        public string Decription { get; set; } = string.Empty;
    }
}