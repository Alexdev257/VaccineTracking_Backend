using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class Child
{
    public int Id { get; set; }

    public int ParentId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public int Gender { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual User Parent { get; set; } = null!;

    public virtual ICollection<VaccinesTracking> VaccinesTrackings { get; set; } = new List<VaccinesTracking>();
}
