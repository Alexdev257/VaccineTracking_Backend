using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class BookingChildId
{
    public int BookingId { get; set; }

    public int ChildId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Child Child { get; set; } = null!;
}
