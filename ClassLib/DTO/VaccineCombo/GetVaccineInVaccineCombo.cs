namespace ClassLib.DTO.VaccineCombo
{
    public class GetVaccineInVaccineCombo
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public int SuggestAgeMin { get; set; }

        public int SuggestAgeMax { get; set; }
    }
}
