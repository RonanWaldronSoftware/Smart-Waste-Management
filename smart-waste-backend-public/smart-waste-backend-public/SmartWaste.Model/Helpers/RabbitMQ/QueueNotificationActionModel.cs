using SmartWaste.Model.Core;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Helpers.FcmNotification;

namespace SmartWaste.Model.Helpers.RabbitMQ
{
    public class QueueNotificationActionModel
    {
        public EmailRabbitMqMessage? Email { get; set; }
        public SmsMessage? Sms { get; set; }
        public FcmPushNotificationModel? FcmPushNotification { get; set; }
    }

    /// <summary>
    /// Model for sending email data into que
    /// </summary>
    public class EmailRabbitMqMessage
    {
        public string SendTo { get; set; }
        public EmailTemplateEnum? Template { get; set; }
        public string Subject { get; set; }
        public string? Message { get; set; }
        public List<string> UrlAttachments { get; set; } = new List<string>();
        /// <summary>
        /// User for pdf binding
        /// </summary>
        public User User { get; set; }
    }

    /// <summary>
    /// Model for sending sms data into que
    /// </summary>
    public class SmsMessage
    {
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
