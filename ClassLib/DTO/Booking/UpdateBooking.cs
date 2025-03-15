namespace ClassLib.DTO.Booking
{
    public class UpdateBooking
    {
        public int BookingId { get; set; }
        public List<int> VaccinesList { get; set; }
        public List<int> VaccinesCombo { get; set; }
    }
}
