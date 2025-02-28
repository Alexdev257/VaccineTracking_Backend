using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.VaccineTracking
{
    public class Test
    {
        public AddVaccinesTrackingRequest AddVaccinesTrackingRequest { get; set; }
        public List<int> vaccineIds { get; set; }
        public List<int> childrenIds { get; set; }
        public List<int> vaccineComboIds { get; set; }
        
    }
}