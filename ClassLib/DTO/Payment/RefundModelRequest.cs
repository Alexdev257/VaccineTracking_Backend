using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.Payment
{
    public class RefundModelRequest
    {
        public string BookingID {get; set;} = string.Empty;
        public double Amount {get; set;} = 0;
    }
}