using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class Payment
{
    public string PaymentId { get; set; } = null!;

    public int BookingId { get; set; }

    public string TransactionId { get; set; } = null!;

    public string PayerId { get; set; } = null!;

    public int PaymentMethod { get; set; }

    public string Currency { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public DateTime PaymentDate { get; set; }

    public string Status { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual PaymentMethod PaymentMethodNavigation { get; set; } = null!;
}
