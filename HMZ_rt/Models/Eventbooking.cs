using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Eventbooking
{
    public int EventBookingId { get; set; }

    public int EventId { get; set; }

    public int GuestId { get; set; }

    public DateTime? BookingDate { get; set; }

    public int? NumberOfTickets { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? Status { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? DateAdded { get; set; }

    public string? Notes { get; set; }
    [JsonIgnore]
    public virtual Event Event { get; set; } = null!;
    [JsonIgnore]
    public virtual Guest Guest { get; set; } = null!;
}
