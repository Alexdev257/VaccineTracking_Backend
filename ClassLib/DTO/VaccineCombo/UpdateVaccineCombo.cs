using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.Models;

namespace ClassLib.DTO.VaccineCombo
{
    public class UpdateVaccineCombo
    {
        public string ComboName { get; set; } = null!;

        public int Discount { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal FinalPrice { get; set; }

        public string Status { get; set; } = null!;

        public List<int> vaccineIds{ get; set; } = null!;

    }
}
