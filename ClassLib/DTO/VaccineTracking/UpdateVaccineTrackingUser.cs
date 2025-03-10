using System.ComponentModel;

namespace ClassLib.DTO.VaccineTracking
{
    public class UpdateVaccineTrackingUser
    {
        public string Reaction { get; set; } = string.Empty;

        [DefaultValue(false)]
        public bool isCancel { get; set; } = false;
        public DateTime? Reschedule { get; set; } = null!;
    }
}