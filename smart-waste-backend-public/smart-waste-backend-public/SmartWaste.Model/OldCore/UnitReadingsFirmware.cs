using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class UnitReadingsFirmware
{
    public int Id { get; set; }

    public int? RawDataId { get; set; }

    public string? Source { get; set; }

    public int? UnitId { get; set; }

    public string? Firmware { get; set; }

    public int? TimeDiff { get; set; }

    public ulong? BinJustOn { get; set; }

    public ulong? RegularPeriodicReporting { get; set; }

    public ulong? NbiotIssue { get; set; }

    public int? HardwareRevision { get; set; }

    public DateTime? InsertDate { get; set; }
}
