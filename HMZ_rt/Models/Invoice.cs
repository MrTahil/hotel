using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HMZ_rt.Models;

public partial class Invoice
{
    public string? Status { get; set; }

    public int InvoiceId { get; set; }

    public int BookingId { get; set; }

    public DateTime? InvoiceDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? DueDate { get; set; }

    public string? Notes { get; set; }

    public DateTime? DateAdded { get; set; }

    public string? Currency { get; set; }
    [JsonIgnore]
    public virtual Booking Booking { get; set; } = null!;
}
