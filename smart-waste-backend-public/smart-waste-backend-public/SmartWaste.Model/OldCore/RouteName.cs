using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class RouteName
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? OwnerId { get; set; }

    public int? InsertBy { get; set; }

    public DateTime? InsertDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual User? Owner { get; set; }
}
