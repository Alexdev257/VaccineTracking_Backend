using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.Payment
{
    public class PaymentResponseStaff
    {
        public string paymentId { get; set; } = string.Empty;
        public string paymentName { get; set; } = string.Empty;
        public decimal amount { get; set; }
    }
}