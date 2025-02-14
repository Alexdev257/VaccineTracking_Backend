using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int PaymentMethod { get; set; }

    public DateTime PaymentDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Booking IdNavigation { get; set; } = null!;

    public virtual PaymentMethod PaymentMethodNavigation { get; set; } = null!;
}
