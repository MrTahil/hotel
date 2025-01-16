using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HMZ_rt.Models;

public partial class Useraccount
{
    public string? Username { get; set; }

    public int UserId { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public string? Role { get; set; }

    public string? Status { get; set; }
    
    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string? Notes { get; set; }
    
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
