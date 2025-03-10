using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Taxrate
{
    public int TaxRateId { get; set; }

    public string? TaxName { get; set; }

    public decimal? Rate { get; set; }

    public DateTime? EffectiveDate { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public DateTime? DateAdded { get; set; }

    public string? Country { get; set; }

    public string? State { get; set; }

    public string? City { get; set; }

    public int PaymentId { get; set; }

    public virtual Payment Payment { get; set; } = null!;
}
