using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.Payment
{
    public class RefundModel
    {
        public int amount { get; set; } = 0;
        public string paymentID { get; set; } = string.Empty;
        public string payerID { get; set; } = string.Empty;
        public string paymentDate { get; set; } = string.Empty;
    }
}