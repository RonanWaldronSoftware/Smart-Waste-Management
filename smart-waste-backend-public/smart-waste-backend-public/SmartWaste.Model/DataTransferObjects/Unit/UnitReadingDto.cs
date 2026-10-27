namespace SmartWaste.Model.DataTransferObjects.Unit
{
    public class UnitReadingDto
    {
        public string SerialNumber { get; set; } = null!;
        public int? BinLevelBc { get; set; }
        public int? BinLevel { get; set; }
        public int? BinLevelPercentage { get; set; }
        public int? BinLevelBcPercentage { get; set; }
        public double? BatteryVoltage { get; set; }
        public bool BatteryUvlo { get; set; }
        public int? MessageType { get; set; }
        public decimal? Rssi { get; set; }
        public double? Rsrq { get; set; }
        public int? Rsrp { get; set; }
        public int? Snr { get; set; }
        public int? Src { get; set; }
        public int? TimeDiff { get; set; }
        public bool NbloTissue { get; set; }
        public bool RegularPeriodicReporting { get; set; }
        public int? NoCompactions { get; set; }
        public int? NoFlapOpenings { get; set; }
        public bool UnitJustOn { get; set; }
        public string? Firmware { get; set; }
        public int? NblotSignalStrength { get; set; }

    }

    public class ShortUnitReadingDto
    {
        public int Id { get; set; }
        public int? BinLevelBc { get; set; }
        public int? BinLevel { get; set; }
        public int? NoFlapOpenings { get; set; }
        public DateTime? ReadingDate { get; set; }
        public double? Rsrq { get; set; }
        public int? NblotSignalStrength { get; set; }
        public double? BatteryVoltage { get; set; }
        public int? Temperature { get; set; }
        public int? NoCompactions { get; set; }
        public List<ShortAlertTypeDto> AlertTypes { get; set; } = new();
    }
}
