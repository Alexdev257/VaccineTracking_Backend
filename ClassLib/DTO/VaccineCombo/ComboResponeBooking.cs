using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.DTO.VaccineCombo
{
    public class ComboResponeBooking
    {
        public int ID { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public int Discount { get; set; } = 0;
        public int totalPrice { get; set; } = 0;
        public int finalPrice { get; set; } = 0;

    }
}