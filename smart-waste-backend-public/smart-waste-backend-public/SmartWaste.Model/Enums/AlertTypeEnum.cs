namespace SmartWaste.Model.Enums
{
    public enum AlertTypeEnum
    {
        #region Warning
        BatteryUVLO = 1,
        EmptiedLastPeriod = 2,
        BatteryOverTemperature = 3,
        BinLocked = 4,
        BinTilted = 5,
        ServiceDoorStuckOpen = 6,
        FlapStuckOpen = 7,
        BinFull = 8,
        DamageReport = 9,
        DeviceNotReporting = 10,

        #endregion

        #region Send

        Sms = 11,
        Email = 12,
        Push = 13,

        #endregion
    }
}
