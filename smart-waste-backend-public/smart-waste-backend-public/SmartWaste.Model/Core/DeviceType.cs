using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class DeviceType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<DeviceToken> DeviceTokens { get; set; } = new List<DeviceToken>();
}
