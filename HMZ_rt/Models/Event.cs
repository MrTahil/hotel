using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Event
{
    public int? Capacity { get; set; }

    public string? Status { get; set; }

    public int EventId { get; set; }

    public string? EventName { get; set; }

    public DateTime? EventDate { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }

    public DateTime? DateAdded { get; set; }

    public string? OrganizerName { get; set; }

    public string? ContactInfo { get; set; }

    public string Images { get; set; } = null!;

    public virtual ICollection<Eventbooking> Eventbookings { get; set; } = new List<Eventbooking>();
}
