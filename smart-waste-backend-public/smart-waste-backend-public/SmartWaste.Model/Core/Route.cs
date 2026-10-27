using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class Route
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? UserId { get; set; }

    public int? CreateUserId { get; set; }

    public DateTime CreateDate { get; set; }

    public int? UpdateUserId { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool Deleted { get; set; }

    public virtual User? CreateUser { get; set; }

    public virtual ICollection<RouteUnit> RouteUnits { get; set; } = new List<RouteUnit>();

    public virtual User? UpdateUser { get; set; }

    public virtual User? User { get; set; }
}
