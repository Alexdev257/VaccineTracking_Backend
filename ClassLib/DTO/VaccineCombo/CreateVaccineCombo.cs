using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.VaccineCombo
{
    public class CreateVaccineCombo
    {
        public int Id { get; set; }

        public string ComboName { get; set; } = null!;

        public int Disount { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal FinalPrice { get; set; }

        public string Status { get; set; } = null!;
    }
}
