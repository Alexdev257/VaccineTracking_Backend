using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.Booking
{
    public class GetBookingForUser
    {
        public int TotalPrice { get; set; }
        
        public string AdvisoryDetail { get; set; } = null!;

        public DateOnly CreatedAt { get; set; }

        public DateOnly ArrivedAt { get; set; }

        public string Status { get; set; } = null!;

        public List<int>? ChildrenIds { get; set; }

        public List<int>? vaccineIds { get; set; }

        public List<int>? vaccineComboIds { get; set; }
    }
}