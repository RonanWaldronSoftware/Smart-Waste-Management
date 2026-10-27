using SmartWaste.Model.DataTransferObjects;

namespace SmartWaste.Model.Helpers.RabbitMQ
{
    public class QueueUnitDataActionModel
    {
        /// <summary>
        /// Data for other bins, not specifically for Sigfox.
        /// </summary>
        public DataSensorModel? DataSensor { get; set; }

        /// <summary>
        /// Model specifically for Sigfox data.
        /// </summary>
        public SigFoxData? SigFoxData { get; set; }
    }

    public sealed class SigFoxData
    {
        public string SerialNumber { get; set; }
        public SigfoxBody SigfoxBody { get; set; } = null!;
    }

    public class DataSensorModel
    {
        /// <summary>
        /// General binary data.
        /// </summary>
        public byte[]? Data { get; set; }

        /// <summary>
        /// Type of sensor associated with the data.
        /// </summary>
        public SensorEnum Sensor { get; set; }
    }

    public enum SensorEnum
    {
        Tekelek = 1,
        SmartWaste = 2,
    }
}
