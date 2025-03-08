using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.Helpers;
using TimeProvider = ClassLib.Helpers.TimeProvider;

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