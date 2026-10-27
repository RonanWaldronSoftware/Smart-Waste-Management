using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class RefCountry
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Abbr { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
