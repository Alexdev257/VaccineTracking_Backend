using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.PaymentMethod
{
    public class UpdatePaymentMethod
    {
        public string Name { get; set; } = null!;

        public string Decription { get; set; } = null!;
    }
}