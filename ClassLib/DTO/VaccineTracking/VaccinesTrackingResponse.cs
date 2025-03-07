using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.VaccineTracking
{
    public class VaccinesTrackingResponse
    {
        public int TrackingID {get; set;} = 0;
        public string VaccineName { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string ChildName { get; set; } = null!;

        public DateTime? MinimumIntervalDate { get; set; }

        public DateTime? VaccinationDate { get; set; }

        public DateTime? MaximumIntervalDate { get; set; }

        public int PreviousVaccination {get; set;} = 0;

        public string Status { get; set; } = null!;

        public string AdministeredByDoctorName { get; set; } = null!;

        public string Reaction { get; set; } = null!;

    }
}