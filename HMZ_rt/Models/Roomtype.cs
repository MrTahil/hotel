using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Roomtype
{
    public string? TypeName { get; set; }

    public int RoomTypeId { get; set; }

    public string? Description { get; set; }

    public decimal? BasePrice { get; set; }

    public int? MaxCapacity { get; set; }

    public string? Amenities { get; set; }

    public string? Status { get; set; }

    public DateTime? DateAdded { get; set; }

    public string? ImageUrl { get; set; }

    public int? Priority { get; set; }
}
