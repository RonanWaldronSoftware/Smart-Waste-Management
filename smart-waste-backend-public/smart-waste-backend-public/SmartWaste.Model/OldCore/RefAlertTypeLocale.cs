using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class RefAlertTypeLocale
{
    public int Id { get; set; }

    public string Locale { get; set; } = null!;

    public string? Name { get; set; }
}
