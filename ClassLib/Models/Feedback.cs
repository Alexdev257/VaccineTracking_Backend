namespace ClassLib.Models;

public partial class Feedback
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RatingScore { get; set; }

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User User { get; set; } = null!;
}
