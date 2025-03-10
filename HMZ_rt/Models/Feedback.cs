using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public DateTime? FeedbackDate { get; set; }

    public string? Comments { get; set; }

    public string? Category { get; set; }

    public decimal? Rating { get; set; }

    public string? Status { get; set; }

    public string? Response { get; set; }

    public DateTime? ResponseDate { get; set; }

    public DateTime? DateAdded { get; set; }

    public int GuestId { get; set; }

    public virtual Guest Guest { get; set; } = null!;
}
