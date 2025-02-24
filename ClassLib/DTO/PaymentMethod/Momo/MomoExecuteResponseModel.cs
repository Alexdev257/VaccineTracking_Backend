using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.Payment
{
    public class MomoExecuteResponseModel
    {
        public string OrderId { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string OrderInfo { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
