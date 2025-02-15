using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.Booking
{
    public class UpdateBooking
    {
        public int Id { get; set; }
        public required string Status { get; set; }
    }
}
