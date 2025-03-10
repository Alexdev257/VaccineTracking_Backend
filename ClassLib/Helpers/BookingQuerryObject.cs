namespace ClassLib.Helpers
{
    // For searching and filtering
    public class BookingQuerryObject
    {
        public int? Id { get; set; }

        public int? ParentId { get; set; }

        public string? Status { get; set; }

        public DateTime? CreateDate { get; set; }

        public Boolean isDescending { get; set; }

        public string? PhoneNumber { get; set; }

        public string? orderBy { get; set; }

    }
}