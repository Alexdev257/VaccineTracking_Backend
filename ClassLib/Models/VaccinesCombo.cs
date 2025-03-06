using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class VaccinesCombo
{
    public int Id { get; set; }

    public string ComboName { get; set; } = null!;

    public int Discount { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal FinalPrice { get; set; }

    public string Status { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Vaccines> Vaccines { get; set; } = new List<Vaccines>();
}
