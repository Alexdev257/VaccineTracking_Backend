using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.Payment
{
    public class MomoCreatePaymentResponeModel
    {
        public string RequestID { get; set; }
        public string ErrorCode { get; set; }
        public string OrderID { get; set; }
        public string Message { get; set; }
        public string LocalMessage { get; set; }
        public string RequestType { get; set; }
        public string PayUrl { get; set; }
        public string Signature { get; set; }
        public string QrCodeUrl { get; set; }
        public string DeepLink { get; set; }
        public string DeepLinkWebInApp { get; set; }
    }
}
