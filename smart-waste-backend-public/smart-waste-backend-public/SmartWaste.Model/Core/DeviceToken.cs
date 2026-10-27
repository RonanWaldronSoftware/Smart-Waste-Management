using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class DeviceToken
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? DeviceTypeId { get; set; }

    public string? DeviceToken1 { get; set; }

    public virtual DeviceType? DeviceType { get; set; }

    public virtual User? User { get; set; }
}
