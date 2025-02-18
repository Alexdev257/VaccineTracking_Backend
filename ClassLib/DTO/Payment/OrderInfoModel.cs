using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.Payment
{
    public class OrderInfoModel
    {
        public string FullName { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public double Amount { get; set; }
    }
}