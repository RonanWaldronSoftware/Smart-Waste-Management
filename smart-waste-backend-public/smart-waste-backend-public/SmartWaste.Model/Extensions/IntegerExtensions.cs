namespace SmartWaste.Model.Extensions
{
    public static class IntegerExtensions
    {
        /// <summary>
        /// Convert battery voltage from unit reading to unit voltage voltage
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double? ConvertBatteryVoltage(this int? value) =>
            value == null ? null : Math.Round(value.Value * 0.05 + 7, 2);
    }
}
