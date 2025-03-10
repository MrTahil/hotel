using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Roommaintenance
{
    public int MaintenanceId { get; set; }

    public int RoomId { get; set; }

    public DateTime? MaintenanceDate { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public int? StaffId { get; set; }

    public DateTime? DateReported { get; set; }

    public DateTime? ResolutionDate { get; set; }

    public decimal? Cost { get; set; }

    public string? Notes { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual Staff? Staff { get; set; }
}
