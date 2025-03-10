namespace ClassLib.DTO.User
{
    public class CreateStaffRequest
    {
        //public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Gmail { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string Avatar { get; set; } = null!;
        public int Gender { get; set; }
        //public string Role { get; set; } = null!;
        //public DateTime CreatedAt { get; set; }
        //public string Status { get; set; } = null!;
        //public bool IsDeleted { get; set; }
    }
}
