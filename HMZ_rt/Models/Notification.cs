using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public DateTime? DateSent { get; set; }

    public string? Message { get; set; }

    public string? Status { get; set; }

    public string? Type { get; set; }

    public DateTime? DateRead { get; set; }

    public int? Priority { get; set; }

    public string? Notes { get; set; }

    public int UserId { get; set; }

    public string? Category { get; set; }

    public virtual Useraccount User { get; set; } = null!;
}
