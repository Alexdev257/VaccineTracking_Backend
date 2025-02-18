using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.User
{
    public class LoginResponse
    {
        //public int Id { get; set; }
        //public string Name { get; set; } = null!;
        //public string PhoneNumber { get; set; } = null!;
        //public string Username { get; set; } = null!;
        ////public string Password { get; set; } = null!;
        //public string Role { get; set; } = null!;
        //public int Gender { get; set; }
        //public string Token { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
