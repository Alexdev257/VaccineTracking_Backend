namespace ClassLib.DTO.Payment
{
    public class OrderInfoModel
    {
        public string GuestName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public string GuestPhone { get; set; } = string.Empty;
        public string BookingID { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string OrderDescription { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0;
    }
}