using System;
using System.Collections.Generic;

namespace HMZ_rt.Models;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public int PublishedYear { get; set; }

    public string? Genre { get; set; }

    public decimal? Price { get; set; }
}
