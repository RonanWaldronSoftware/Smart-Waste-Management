using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class UnitLevelType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? TranslationKey { get; set; }
}
