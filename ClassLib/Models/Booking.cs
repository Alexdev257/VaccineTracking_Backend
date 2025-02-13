using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int ParentId { get; set; }

    public int ChildId { get; set; }

    public string AdvisoryDetail { get; set; } = null!;

    public int TotalPrice { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly ArrivedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual Child Child { get; set; } = null!;

    public virtual User Parent { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
