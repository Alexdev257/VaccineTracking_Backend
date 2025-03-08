using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Gmail { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string Avatar { get; set; } = null!;

    public int Gender { get; set; }

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Child> Children { get; set; } = new List<Child>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<VaccinesTracking> VaccinesTrackings { get; set; } = new List<VaccinesTracking>();
}
