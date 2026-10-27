using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class RefBinType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? EmptyLevel { get; set; }

    public int? FullLevel { get; set; }

    public int? Capacity { get; set; }

    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
}
