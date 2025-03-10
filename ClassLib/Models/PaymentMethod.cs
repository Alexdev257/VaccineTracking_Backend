namespace ClassLib.Models;

public partial class PaymentMethod
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
