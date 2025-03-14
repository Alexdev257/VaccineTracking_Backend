using ClassLib.DTO.Child;
using ClassLib.DTO.Vaccine;
using ClassLib.DTO.VaccineCombo;

namespace ClassLib.DTO.Booking
{
    public class BookingResponse
    {
        public int ID { get; set; } = 0;
        public string AdvisoryDetail { get; set; } = null!;

        public DateTime ArrivedAt { get; set; }

        public string paymentName { get; set; } = string.Empty;

        public decimal Amount { get; set; } = 0;

        public string Status { get; set; } = string.Empty;

        public List<ChildrenResponeBooking>? ChildrenList { get; set; }

        public List<VaccineResponeBooking>? VaccineList { get; set; }

        public List<ComboResponeBooking>? ComboList { get; set; }
    }
}