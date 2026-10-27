using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class UnitReadingsTemp
{
    public int Id { get; set; }

    public int? UnitId { get; set; }

    public string? SerialNo { get; set; }

    public int? RawDataId { get; set; }

    public string? Source { get; set; }

    public byte? MsgType { get; set; }

    public byte? BinLevelBc { get; set; }

    public ushort? BinLevel { get; set; }

    public uint? NoFlapOpenings { get; set; }

    public byte? BatteryVoltage { get; set; }

    public sbyte? Temperature { get; set; }

    public byte? NoCompactions { get; set; }

    public ulong? BatteryUvlo { get; set; }

    public ulong? BinEmptiedLastPeriod { get; set; }

    public ulong? BatteryOverTempLo { get; set; }

    public ulong? BinLocked { get; set; }

    public ulong? BinFull { get; set; }

    public ulong? BinTilted { get; set; }

    public ulong? ServiceDoorOpen { get; set; }

    public ulong? FlapStuckOpen { get; set; }

    public ulong? ServiceDoorClosed { get; set; }

    public bool? NbIotSignalStrength { get; set; }

    /// <summary>
    /// Received Signal Strength Indicator
    /// </summary>
    public double? Rssi { get; set; }

    /// <summary>
    /// Sonic Result Code
    /// </summary>
    public short? Src { get; set; }

    public sbyte? ContactReason { get; set; }

    public sbyte? AlarmStatus { get; set; }

    /// <summary>
    /// Signal to Noise Ratio (Sigfox sends this)
    /// </summary>
    public double? Snr { get; set; }

    /// <summary>
    /// ber - SmartWaste units send this
    /// </summary>
    public short? Ber { get; set; }

    public double? Rsrq { get; set; }

    public int? Rsrp { get; set; }

    public string? Firmware { get; set; }

    public int? TimeDiff { get; set; }

    public ulong? BinJustOn { get; set; }

    public ulong? RegularPeriodicReporting { get; set; }

    public ulong? NbIoTissue { get; set; }

    public DateTime? ReadingDateTime { get; set; }

    public DateTime? InsertDateTime { get; set; }
}
