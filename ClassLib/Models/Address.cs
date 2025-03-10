namespace ClassLib.Models;

public partial class Address
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual ICollection<Vaccine> Vaccines { get; set; } = new List<Vaccine>();
}
