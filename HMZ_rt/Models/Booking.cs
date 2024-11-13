using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Booking
{
    public int RoomId { get; set; }

    public int BookingId { get; set; }

    public int GuestId { get; set; }

    public DateTime? CheckInDate { get; set; }

    public DateTime? CheckOutDate { get; set; }

    public int? NumberOfGuests { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? BookingDate { get; set; }

    public string? Status { get; set; }

    public string? PaymentStatus { get; set; }

    public virtual Guest Guest { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Room Room { get; set; } = null!;
}
