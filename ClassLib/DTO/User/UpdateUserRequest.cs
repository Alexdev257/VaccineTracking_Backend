using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.User
{
    public class UpdateUserRequest
    {
        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public int gender { get; set; }
        public string Gmail { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
