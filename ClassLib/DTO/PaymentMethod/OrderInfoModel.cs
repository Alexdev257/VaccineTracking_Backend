using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.Payment
{
    public class OrderInfoModel
    {
        public string FullName { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string OrderInfo { get; set; } = string.Empty;
        public double Amount { get; set; }
    }
}