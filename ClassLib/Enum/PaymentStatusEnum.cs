namespace ClassLib.Enum
{
    public enum PaymentStatusEnum
    {
        Success,
        Pending,
        Refunded,
        FullyRefunded = 1,
        PartialRefunded = 0
    }
}