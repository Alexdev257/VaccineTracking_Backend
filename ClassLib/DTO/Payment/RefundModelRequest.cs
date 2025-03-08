using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.Enum;

namespace ClassLib.DTO.Payment
{
    public class RefundModelRequest
    {
        public string BookingID {get; set;} = string.Empty;
        public int paymentStatusEnum {get;set;} = 0;
    }
}