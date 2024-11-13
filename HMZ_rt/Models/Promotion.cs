using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Promotion
{
    public int PromotionId { get; set; }

    public string? PromotionName { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public string? TermsConditions { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public int? RoomId { get; set; }

    public string? Status { get; set; }

    public DateTime? DateAdded { get; set; }

    public virtual Room? Room { get; set; }
}
