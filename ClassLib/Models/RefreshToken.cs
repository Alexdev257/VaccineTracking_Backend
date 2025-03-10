namespace ClassLib.Models;

public partial class RefreshToken
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public string RefreshToken1 { get; set; } = null!;

    public string AccessToken { get; set; } = null!;

    public bool IsUsed { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime IssuedAt { get; set; }

    public DateTime ExpiredAt { get; set; }

    public virtual User User { get; set; } = null!;
}
