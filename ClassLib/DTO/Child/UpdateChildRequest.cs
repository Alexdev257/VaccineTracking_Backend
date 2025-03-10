namespace ClassLib.DTO.Child
{
    public class UpdateChildRequest
    {
        public int ParentId { get; set; }

        public string Name { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public int Gender { get; set; }
        public string Status { get; set; } = null!;
    }
}
