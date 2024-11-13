using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Marketing
{
    public int MarketingId { get; set; }

    public string? CampaignName { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? Budget { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public string? TargetAudience { get; set; }

    public DateTime? DateAdded { get; set; }

    public string? Notes { get; set; }
}
