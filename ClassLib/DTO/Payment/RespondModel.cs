using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentAPI.Model
{
    public class RespondModel
    {
        public string OrderId { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string BookingID { get; set; } = string.Empty;
        public string OrderDescription { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TrancasionID { get; set; } = string.Empty;
    }
}