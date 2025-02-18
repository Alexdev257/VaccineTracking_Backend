using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class BookingComboId
{
    public int BookingId { get; set; }

    public int ComboId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual VaccinesCombo Combo { get; set; } = null!;
}
