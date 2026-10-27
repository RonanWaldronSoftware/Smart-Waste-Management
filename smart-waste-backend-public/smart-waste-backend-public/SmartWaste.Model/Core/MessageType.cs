using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class MessageType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? TranslationKey { get; set; }

    public virtual ICollection<UnitReading> UnitReadings { get; set; } = new List<UnitReading>();
}
