using System;
using System.Collections.Generic;

namespace SmartWaste.Model.OldCore;

public partial class LatestReadingsAll
{
    public int Id { get; set; }

    public int UnitId { get; set; }

    public int SerialNo { get; set; }

    public int RawDataId { get; set; }

    public int Source { get; set; }

    public int MsgType { get; set; }

    public int BinLevelBc { get; set; }

    public int BinLevel { get; set; }

    public int NoFlapOpenings { get; set; }

    public int BatteryVoltage { get; set; }

    public int Temperature { get; set; }

    public int NoCompactions { get; set; }

    public int BatteryUvlo { get; set; }

    public int BinEmptiedLastPeriod { get; set; }

    public int BatteryOverTempLo { get; set; }

    public int BinLocked { get; set; }

    public int BinFull { get; set; }

    public int BinTilted { get; set; }

    public int ServiceDoorOpen { get; set; }

    public int FlapStuckOpen { get; set; }

    public int ServiceDoorClosed { get; set; }

    public int NbIotSignalStrength { get; set; }

    public int Rssi { get; set; }

    public int Src { get; set; }

    public int Snr { get; set; }

    public int Ber { get; set; }

    public int Rsrq { get; set; }

    public int Rsrp { get; set; }

    public int Firmware { get; set; }

    public int TimeDiff { get; set; }

    public int BinJustOn { get; set; }

    public int RegularPeriodicReporting { get; set; }

    public int NbIoTissue { get; set; }

    public int ReadingDateTime { get; set; }

    public int InsertDateTime { get; set; }
}
