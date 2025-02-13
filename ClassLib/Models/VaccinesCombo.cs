using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class VaccinesCombo
{
    public int Id { get; set; }

    public string ComboName { get; set; } = null!;

    public int Discount { get; set; }

    public int TotalPrice { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual ICollection<Vaccine> Vaccines { get; set; } = new List<Vaccine>();
}
