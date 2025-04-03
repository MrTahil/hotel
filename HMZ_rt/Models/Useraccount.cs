using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Useraccount
{
    public string? Username { get; set; }

    public int UserId { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public string? Role { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public string? Status { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string? Notes { get; set; }

    public string? Authenticationcode { get; set; }

    public DateTime? Authenticationexpire { get; set; }

    public virtual ICollection<Guest> Guests { get; set; } = new List<Guest>();

    public virtual ICollection<Newsletter> Newsletters { get; set; } = new List<Newsletter>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
