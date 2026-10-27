using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class RawReading
{
    public int Id { get; set; }

    public string? Source { get; set; }

    public byte[]? RawData { get; set; }

    public DateTime? InsertAt { get; set; }

    public virtual ICollection<UnitReading> UnitReadings { get; set; } = new List<UnitReading>();
}
