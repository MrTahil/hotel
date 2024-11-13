using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Staff
{
    public string? FirstName { get; set; }

    public int StaffId { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Position { get; set; }

    public decimal? Salary { get; set; }

    public DateTime? DateHired { get; set; }

    public string? Status { get; set; }

    public string? Department { get; set; }

    public virtual ICollection<Roommaintenance> Roommaintenances { get; set; } = new List<Roommaintenance>();
}
