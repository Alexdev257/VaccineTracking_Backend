using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.User
{
    public class ChangePasswordRequest
    {
        public string NewPassword { get; set; } = null!;
    }
}
