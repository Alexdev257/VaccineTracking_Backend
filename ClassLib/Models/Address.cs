using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class Address
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Vaccine> Vaccines { get; set; } = new List<Vaccine>();
}
