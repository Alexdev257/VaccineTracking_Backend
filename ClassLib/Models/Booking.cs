using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int ParentId { get; set; }

    public string AdvisoryDetails { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ArrivedAt { get; set; }

    public string Status { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual User Parent { get; set; } = null!;

    public virtual ICollection<Child> Children { get; set; } = new List<Child>();

    public virtual ICollection<VaccinesCombo> Combos { get; set; } = new List<VaccinesCombo>();

    public virtual ICollection<Vaccine> Vaccines { get; set; } = new List<Vaccine>();
}
