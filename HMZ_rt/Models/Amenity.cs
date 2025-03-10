using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Amenity
{
    public string? AmenityName { get; set; }

    public string? Description { get; set; }

    public int AmenityId { get; set; }

    public string? Availability { get; set; }

    public DateTime? DateAdded { get; set; }

    public int? RoomId { get; set; }

    public string? Status { get; set; }

    public string? Icon { get; set; }

    public string? Category { get; set; }

    public int? Priority { get; set; }

    public virtual Room? Room { get; set; }
}
