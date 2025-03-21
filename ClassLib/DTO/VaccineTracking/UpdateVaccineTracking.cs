namespace ClassLib.DTO.VaccineTracking
{
    public class UpdateVaccineTracking
    {
        public string Status { get; set; } = null!;
        public string Reaction { get; set; } = null!;
        public int AdministeredBy { get; set; } = 0;
        public DateTime? Reschedule { get; set; } = null;
    }
}