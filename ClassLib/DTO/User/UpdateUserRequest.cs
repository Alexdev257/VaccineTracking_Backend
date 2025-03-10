namespace ClassLib.DTO.User
{
    public class UpdateUserRequest
    {
        //public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public int Gender { get; set; }
        public string Avatar { get; set; } = null!;

        public string Gmail { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
