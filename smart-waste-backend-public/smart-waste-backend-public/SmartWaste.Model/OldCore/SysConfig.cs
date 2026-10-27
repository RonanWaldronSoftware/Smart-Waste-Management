using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class SysConfig
{
    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
