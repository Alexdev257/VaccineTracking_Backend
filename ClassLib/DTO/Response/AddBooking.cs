using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.Models;

namespace ClassLib.DTO.Response
{
    internal class AddBooking
    {
        public int ParentId { get; set; }
        public required DateTime BookingDate { get; set; }

        public int TotalPrice { get; set; }

        public ICollection<Child>? Children { get; set; }

        public ICollection<Vaccine>? Vaccines { get; set; }

        public ICollection<VaccinesCombo>?  VaccinesCombos { get; set; }
    }
}
