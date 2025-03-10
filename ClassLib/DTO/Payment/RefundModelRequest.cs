namespace ClassLib.DTO.Payment
{
    public class RefundModelRequest
    {
        public string BookingID {get; set;} = string.Empty;
        public int paymentStatusEnum {get;set;} = 0;
    }
}