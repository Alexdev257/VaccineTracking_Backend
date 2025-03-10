namespace ClassLib.DTO.Email
{
    public class EmailRequest
    {
        public string ToEmail { get; set; }        // Địa chỉ email người nhận
        public string Subject { get; set; }   // Tiêu đề email
        public string UserName { get; set; }  // Tên người dùng (dùng trong template)
        public string VerifyCode { get; set; } // Mã xác thực (dùng trong template)
    }
}
