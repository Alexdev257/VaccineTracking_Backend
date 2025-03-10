using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.Payment
{
    public class RefundModel
    {
            public double amount { get; set; } = 0;
            public string paymentID { get; set; } = string.Empty;
            public string payerID { get; set; } = string.Empty;
            public DateTime paymentDate { get; set; } = DateTime.Now;
            public string currency { get; set; } = string.Empty;
            public string trancasionID { get; set; } = string.Empty;
            public int RefundType { get; set; } = 0;
        }
}