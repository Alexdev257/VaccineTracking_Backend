using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.DTO.Vaccine;


namespace ClassLib.DTO.VaccineCombo
{
    public class CreateVaccineCombo
    {

        public string ComboName { get; set; } = null!;

        public int Discount { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal FinalPrice => TotalPrice * (1 - (Discount / 100m));

        public string Status { get; set; } = null!;

        public List<VaccineIds> vaccines { get; set; }
    }
}
