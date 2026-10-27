using Newtonsoft.Json;

namespace SmartWaste.Model.Helpers.FcmNotification
{
    /// <summary>
    /// Firebase cloud messaging for push notification
    /// </summary>
    public class FcmPushNotificationModel
    {
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }

        /// <summary>
        /// 1- andorid, 2- ios
        /// </summary>
        [JsonProperty("deviceType")]
        public int DeviceType { get; set; }
    }
}
