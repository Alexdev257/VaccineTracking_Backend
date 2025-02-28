using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.VaccineTracking
{
    public class AddVaccinesTrackingRequest
    {
        public int UserId { get; set; }

        public DateTime? VaccinationDate { get; set; }

        public int AdministeredBy { get; set; }
    }
}