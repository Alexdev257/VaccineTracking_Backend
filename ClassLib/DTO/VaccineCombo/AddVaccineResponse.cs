using ClassLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.VaccineCombo
{
    public class AddVaccineResponse
    {
        public int ComboId { get; set; }

        public string ComboName { get; set; } = null!;

        public List<Models.Vaccines> Vaccines { get; set; }
    }
}
