using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class UnitCategory
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<UnitImage> UnitImages { get; set; } = new List<UnitImage>();
}
