using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.VaccineCombo
{
    public class GetAllVaccineCombo
    {
        public int Id { get; set; }
        public string ComboName { get; set; } = null!;

        public int Discount { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal FinalPrice { get; set; }

        public string Status { get; set; } = null!;

        public List<int>? vaccineIds { get; set; }
    }
}
