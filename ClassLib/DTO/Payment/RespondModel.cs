namespace ClassLib.DTO.Payment
{
    public class RespondModel
    {
        public string OrderId { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string BookingID { get; set; } = string.Empty;
        public string OrderDescription { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TrancasionID { get; set; } = string.Empty;
    }
}