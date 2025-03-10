namespace ClassLib.DTO.VaccineTracking
{
    public class AddVaccinesTrackingRequest
    {
        public int UserId { get; set; }

        public DateTime? VaccinationDate { get; set; }

        public int AdministeredBy { get; set; }
    }
}