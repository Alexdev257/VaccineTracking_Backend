using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.User
{
    public class RegisterRequest
    {

        public string Name { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public int Gender { get; set; }
    }
}
