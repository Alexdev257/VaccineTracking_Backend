namespace ClassLib.DTO.User
{
    public class VerifyOtpLoginPhoneRequest
    {
        public string PhoneNumber { get; set; } = null!;
        public string OTP { get; set; } = null!;
    }
}
