namespace ClassLib.DTO.User
{
    public class RegisterResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string DateOfBirth { get; set; }
        public int Gender { get; set; }
        public string Role { get; set; } = null!;
        public string CreatedAt { get; set; }
        public string Status { get; set; } = null!;
        
    }
}
