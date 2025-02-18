using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class BookingIdVaccineId
{
    public int BookingId { get; set; }

    public int VaccineId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Vaccine Vaccine { get; set; } = null!;
}
