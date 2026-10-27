using System;
using System.Collections.Generic;

namespace SmartWaste.Model.Core;

public partial class UnitReading
{
    public int Id { get; set; }

    public int? UnitId { get; set; }

    public string SerialNumber { get; set; } = null!;

    public string? Source { get; set; }

    public int? MessageType { get; set; }

    public int? BinLevelBc { get; set; }

    public int? BinLevel { get; set; }

    public int? NoFlapOpenings { get; set; }

    public int? BatteryVoltage { get; set; }

    public int? Temperature { get; set; }

    public int? NoCompactions { get; set; }

    public bool BatteryUvlo { get; set; }

    public bool BinEmptiedLastPeriod { get; set; }

    public bool BatteryOverTempLo { get; set; }

    public bool BinLocked { get; set; }

    public bool BinFull { get; set; }

    public bool BinTilted { get; set; }

    public bool ServiceDoorOpen { get; set; }

    public bool FlapStuckOpen { get; set; }

    public bool ServiceDoorClosed { get; set; }

    public int? NblotSignalStrength { get; set; }

    public decimal? Rssi { get; set; }

    public int? Src { get; set; }

    public int? ContactReason { get; set; }

    public int? AlarmStatus { get; set; }

    public double? Snr { get; set; }

    public int? Ber { get; set; }

    public double? Rsrq { get; set; }

    public int? Rsrp { get; set; }

    public string? Firmware { get; set; }

    public int? TimeDiff { get; set; }

    public bool UnitJustOn { get; set; }

    public bool RegularPeriodicReporting { get; set; }

    public bool NbloTissue { get; set; }

    public DateTime? ReadingDate { get; set; }

    public DateTime CreateDate { get; set; }

    public int? HardwareRevision { get; set; }

    public int FullnessStatusId { get; set; }

    public virtual Status FullnessStatus { get; set; } = null!;

    public virtual Unit? Unit { get; set; }
}
