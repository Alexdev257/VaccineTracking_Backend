using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.User
{
    public class VerifyOtpLoginPhoneRequest
    {
        public string PhoneNumber { get; set; } = null!;
        public string OTP { get; set; } = null!;
    }
}
