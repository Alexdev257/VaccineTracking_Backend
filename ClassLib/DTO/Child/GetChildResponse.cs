namespace ClassLib.DTO.Child
{
    public class GetChildResponse
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; } = null!;

        public string DateOfBirth { get; set; }

        public int Gender { get; set; }

        public string Status { get; set; } = null!;

        public string CreatedAt { get; set; }

        //public bool IsDeleted { get; set; }
    }
}
