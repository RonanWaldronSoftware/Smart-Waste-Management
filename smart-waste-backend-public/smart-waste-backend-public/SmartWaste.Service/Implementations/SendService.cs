using SmartWaste.Model.Constants;
using SmartWaste.Model.Helpers.FcmNotification;
using SmartWaste.Model.Helpers.RabbitMQ;
using SmartWaste.Service.Contracts;

namespace SmartWaste.Service.Implementations;

public class SendService(IRabbitMQService rabbitMqService) : ISendService
{
    public void SendSms(string message, string phoneNumber)
    {
        // Create message for send
        var notificationModel = new QueueNotificationActionModel
        {
            Sms = new SmsMessage
            {
                Message = message,
                PhoneNumber = phoneNumber
            }
        };
        
        // Set into que and send
        rabbitMqService.SendMessage(notificationModel, GlobalConstants.NotificationQueue);
    }

    public void SendEmail(string message, string subject, string to)
    {
        // Create message for send
        var notificationModel = new QueueNotificationActionModel
        {
            Email = new EmailRabbitMqMessage
            {
                SendTo = to,
                Message = message,
                Subject = subject,              
            }
        };
        
        // Set into que and send
        rabbitMqService.SendMessage(notificationModel, GlobalConstants.NotificationQueue);
    }

    public void SendPushNotification(string message, string title, string deviceToken)
    {
       var notificationModel = new QueueNotificationActionModel
       {
           FcmPushNotification = new FcmPushNotificationModel()
           {
               DeviceId = deviceToken,
               Title = title,
               Body = message,
           }
       };

        // Set into que and send
        rabbitMqService.SendMessage(notificationModel, GlobalConstants.NotificationQueue);
    }
}