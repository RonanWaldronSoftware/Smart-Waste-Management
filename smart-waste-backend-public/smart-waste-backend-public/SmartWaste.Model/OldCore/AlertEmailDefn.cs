using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class AlertEmailDefn
{
    public int AlertType { get; set; }

    public string Locale { get; set; } = null!;

    public string? Subject { get; set; }

    public sbyte? HtmlBody { get; set; }

    public string? Body { get; set; }
}
