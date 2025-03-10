namespace ClassLib.DTO.Booking
{
    public class AddBooking
    {
        public int ParentId { get; set; }

        public string AdvisoryDetail { get; set; } = null!;

        public int TotalPrice { get; set; }

        public DateTime ArrivedAt { get; set; }

        public int paymentId { get; set; }

        public List<int>? ChildrenIds { get; set; }

        public List<int>? vaccineIds { get; set; }

        public List<int>? vaccineComboIds { get; set; }

        public int BookingID { get; set; } = 0;
    }
}