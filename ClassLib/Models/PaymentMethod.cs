using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class PaymentMethod
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Decription { get; set; } = null!;
}
