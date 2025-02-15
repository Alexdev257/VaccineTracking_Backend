using System;
using System.Collections.Generic;

namespace ClassLib.Models;
public partial class BookingDetail
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int ChildId { get; set; }

    public int? ComboId { get; set; }

    public int? VaccineId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Child Child { get; set; } = null!;

    public virtual VaccinesCombo? Combo { get; set; }

    public virtual Vaccine? Vaccine { get; set; }
}
