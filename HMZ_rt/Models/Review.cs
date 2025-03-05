﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HMZ_rt.Models;

public partial class Review
{
    public DateTime? ReviewDate { get; set; }

    public int ReviewId { get; set; }

    public int GuestId { get; set; }

    public int RoomId { get; set; }

    public decimal? Rating { get; set; }

    public string? Comment { get; set; }

    public string? Status { get; set; }

    public string? Response { get; set; }

    public DateTime? ResponseDate { get; set; }

    public DateTime? DateAdded { get; set; }
    [JsonIgnore]
    public virtual Guest Guest { get; set; } = null!;
    [JsonIgnore]
    public virtual Room Room { get; set; } = null!;
}
