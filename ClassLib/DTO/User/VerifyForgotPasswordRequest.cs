using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.User
{
    public class VerifyForgotPasswordRequest
    {
        //public string Username { get; set; } = null!;
        //public string newPassword { get; set; } = null!;
        public string VerifyCode { get; set; } = null!;
    }
}
