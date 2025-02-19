using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.Payment
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string OrderDescription { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}