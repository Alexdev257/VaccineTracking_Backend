using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.BookingDetail
{
    public class AddBookingDetails
    {
        public int BookingID;

        public List<int>? ChildrenIds { get; set; }

        public List<int>? vaccineIds { get; set; }

        public List<int>? vaccineComboIds { get; set; }
    }
}