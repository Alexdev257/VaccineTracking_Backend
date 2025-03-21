﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.VaccineDTO
{
    public class GetAllvaccineByAdmin
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int Quantity { get; set; }

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int DoesTimes { get; set; }

        public string FromCountry { get; set; } = null!;

        public int SuggestAgeMin { get; set; }

        public int SuggestAgeMax { get; set; }

        public DateTime EntryDate { get; set; }

        public DateTime TimeExpired { get; set; }
        public int? MinimumIntervalDate { get; set; }

        public int? MaximumIntervalDate { get; set; }

        public string Status { get; set; } = null!;

        public int AddressId { get; set; }
        public bool IsDeleted { get; set; }

    }
}
