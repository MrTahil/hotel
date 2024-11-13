using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Loyaltyprogram
{
    public string? ProgramName { get; set; }

    public int LoyaltyProgramId { get; set; }

    public string? Description { get; set; }

    public int? PointsRequired { get; set; }

    public string? Status { get; set; }

    public DateTime? DateAdded { get; set; }

    public string? Benefits { get; set; }

    public int? ExpirationPeriod { get; set; }

    public string? TermsConditions { get; set; }

    public string? Category { get; set; }
}
