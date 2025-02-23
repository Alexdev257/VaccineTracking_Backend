using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.User
{
    public class VerifyOtpRequest
    {
        //public string PhoneNumber { get; set; }
        //public string IdToken { get; set; }

        public string SessionInfo { get; set; }
        public string Otp { get; set; }
    }
}
