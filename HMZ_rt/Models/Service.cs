using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Service
{
    public string? ServiceName { get; set; }

    public int ServiceId { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public string? ServiceType { get; set; }

    public string? Availability { get; set; }

    public DateTime? DateAdded { get; set; }

    public int? Duration { get; set; }

    public int? StaffId { get; set; }

    public decimal? Rating { get; set; }
}
