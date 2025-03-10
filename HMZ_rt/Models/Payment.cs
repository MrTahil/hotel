using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HMZ_rt.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal? Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? TransactionId { get; set; }

    public string? Status { get; set; }

    public string? Currency { get; set; }

    public string? PaymentNotes { get; set; }

    public DateTime? DateAdded { get; set; }
    [JsonIgnore]
    public virtual Booking Booking { get; set; } = null!;

    public virtual ICollection<Taxrate> Taxrates { get; set; } = new List<Taxrate>();
}
