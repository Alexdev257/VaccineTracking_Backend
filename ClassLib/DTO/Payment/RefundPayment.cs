namespace ClassLib.DTO.Payment
{
    public class RefundPayment
    {
        public string PaymentID { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}