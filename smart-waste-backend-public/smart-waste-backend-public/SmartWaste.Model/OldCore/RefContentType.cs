using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class RefContentType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
}
