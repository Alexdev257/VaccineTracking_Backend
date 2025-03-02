using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class Vaccines
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Quantity { get; set; }

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public int DoesTimes { get; set; }

    public int SuggestAgeMin { get; set; }

    public int SuggestAgeMax { get; set; }

    public DateTime EntryDate { get; set; }

    public DateTime TimeExpired { get; set; }

    public int AddressId { get; set; }

    public string Status { get; set; } = null!;

    public int? MinimumIntervalDate { get; set; }

    public int? MaximumIntervalDate { get; set; }

    public string FromCountry { get; set; } = null!;

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<VaccinesTracking> VaccinesTrackings { get; set; } = new List<VaccinesTracking>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<VaccinesCombo> VaccinesCombos { get; set; } = new List<VaccinesCombo>();
}
