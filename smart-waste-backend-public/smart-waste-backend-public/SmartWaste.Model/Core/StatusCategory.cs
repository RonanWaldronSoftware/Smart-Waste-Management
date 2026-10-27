using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class StatusCategory
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Status> Statuses { get; set; } = new List<Status>();
}
