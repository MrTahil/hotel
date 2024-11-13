using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Roominventory
{
    public string? ItemName { get; set; }

    public int? Quantity { get; set; }

    public string? Status { get; set; }

    public DateTime? DateAdded { get; set; }

    public DateTime? LastUpdated { get; set; }

    public string? Notes { get; set; }

    public string? Supplier { get; set; }

    public int InventoryId { get; set; }

    public int RoomId { get; set; }

    public decimal? CostPerItem { get; set; }

    public virtual Room Room { get; set; } = null!;
}
