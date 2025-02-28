using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.Booking
{
    public class BookingResponse
    {
        public int ParentId { get; set; }

        public string AdvisoryDetail { get; set; } = null!;

        public DateTime ArrivedAt { get; set; }

        public int paymentId { get; set; }

        public List<int>? ChildrenIds { get; set; }

        public List<int>? vaccineIds { get; set; }

        public List<int>? vaccineComboIds { get; set; }
    }
}