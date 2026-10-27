using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class AlertPushDefn
{
    public int AlertType { get; set; }

    public string Locale { get; set; } = null!;

    public string? Title { get; set; }

    public string? Body { get; set; }
}
