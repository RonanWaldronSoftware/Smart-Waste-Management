using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class RouteUnit
{
    public int RouteId { get; set; }

    public int RouteIndex { get; set; }

    public int UnitId { get; set; }

    public virtual Unit Unit { get; set; } = null!;
}
