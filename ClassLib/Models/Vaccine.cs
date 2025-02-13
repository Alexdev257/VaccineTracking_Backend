using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class Vaccine
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Quantity { get; set; }

    public string Description { get; set; } = null!;

    public string Image { get; set; } = null!;

    public int Price { get; set; }

    public int DoesTimes { get; set; }

    public int SuggestAgeMin { get; set; }

    public int SuggestAgeMax { get; set; }

    public DateTime EntryDate { get; set; }

    public DateTime TimeExpired { get; set; }

    public int AddressId { get; set; }

    public string Status { get; set; } = null!;

    public int MiniumIntervalDate { get; set; }

    public int MaxiumIntervalDate { get; set; }

    public string Country { get; set; } = null!;

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual ICollection<VaccinesTracking> VaccinesTrackings { get; set; } = new List<VaccinesTracking>();

    public virtual ICollection<VaccinesCombo> VaccineCombos { get; set; } = new List<VaccinesCombo>();
}
