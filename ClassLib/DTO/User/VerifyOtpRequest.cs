namespace ClassLib.DTO.User
{
    public class VerifyOtpRequest
    {
        //public string PhoneNumber { get; set; }
        //public string IdToken { get; set; }

        public string SessionInfo { get; set; }
        public string Otp { get; set; }
    }
}
