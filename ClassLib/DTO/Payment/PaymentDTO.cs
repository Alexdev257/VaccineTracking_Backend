namespace ClassLib.DTO.Payment
{
    public class PaymentDTO
    {
        public string OrderId { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string BookingID { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}