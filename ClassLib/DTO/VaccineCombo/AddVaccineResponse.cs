namespace ClassLib.DTO.VaccineCombo
{
    public class AddVaccineResponse
    {
        public int ComboId { get; set; }

        public string ComboName { get; set; } = null!;

        public List<Models.Vaccine> Vaccines { get; set; }
    }
}
