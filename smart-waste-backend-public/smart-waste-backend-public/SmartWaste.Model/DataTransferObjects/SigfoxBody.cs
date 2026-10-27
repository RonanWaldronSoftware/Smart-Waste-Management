namespace SmartWaste.Model.DataTransferObjects
{
    public class SigfoxBody
    {
        public string DeviceID { get; set; }
        public string Snr { get; set; }
        public string Rssi { get; set; }
        public string Data { get; set; }
    }
}
