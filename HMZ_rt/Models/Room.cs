using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Room
{
    public string? RoomType { get; set; }

    public int RoomId { get; set; }

    public string? RoomNumber { get; set; }

    public int? Capacity { get; set; }

    public decimal? PricePerNight { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public int? FloorNumber { get; set; }

    public string? Amenities { get; set; }

    public DateTime? DateAdded { get; set; }

    public virtual ICollection<Amenity> AmenitiesNavigation { get; set; } = new List<Amenity>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Roominventory> Roominventories { get; set; } = new List<Roominventory>();

    public virtual ICollection<Roommaintenance> Roommaintenances { get; set; } = new List<Roommaintenance>();
}
