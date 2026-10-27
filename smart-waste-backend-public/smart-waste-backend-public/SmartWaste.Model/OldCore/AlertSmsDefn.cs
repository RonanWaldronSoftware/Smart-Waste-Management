using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class AlertSmsDefn
{
    public int AlertType { get; set; }

    public string Locale { get; set; } = null!;

    public string? Message { get; set; }
}
