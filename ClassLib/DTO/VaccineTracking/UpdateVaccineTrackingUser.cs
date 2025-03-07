using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.VaccineTracking
{
    public class UpdateVaccineTrackingUser
    {
        public string Reaction { get; set; } = string.Empty;

        [DefaultValue(false)]
        public bool isCancel { get; set; } = false;
    }
}