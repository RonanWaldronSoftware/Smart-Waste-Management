using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class RouteUnit
{
    public int? RouteId { get; set; }

    public int? UnitId { get; set; }

    public int? RouteIndex { get; set; }

    public int Id { get; set; }

    public virtual Route? Route { get; set; }

    public virtual Unit? Unit { get; set; }
}
