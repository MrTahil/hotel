using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HMZ_rt.Models;

public partial class Newsletter
{
    public int Newsid { get; set; }

    public string Email { get; set; } = null!;

    public int Userid { get; set; }
    [JsonIgnore]
    public virtual Useraccount User { get; set; } = null!;
}
