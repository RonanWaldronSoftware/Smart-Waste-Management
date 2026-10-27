using SmartWaste.Model.Configuration;
using SmartWaste.Model.Constants;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Helpers.Email;
using SmartWaste.Model.Helpers.RabbitMQ;
using SmartWaste.Model.Static;
using SmartWaste.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace SmartWaste.Notification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueueSubscriberController : ControllerBase
    {
        private readonly ILogger<QueueSubscriberController> _logger;
        private readonly IWebHostEnvironment _webHostEnviroment;
        private readonly IHttpContextAccessor _accessor;
        private readonly Configuration _configuration;
        private readonly IServiceManager _serviceManager;
        private const string _notificationQueue = GlobalConstants.NotificationQueue;

        // Rabbitmq variables
        private IConnection? _connection;
        private IModel? _channel;
        private EventingBasicConsumer consumer;
        
        public QueueSubscriberController(ILogger<QueueSubscriberController> logger, IHttpContextAccessor accessor, IServiceManager serviceManager, IWebHostEnvironment webHostEnviroment,
                                        IOptions<Configuration> configuration)
        {
            _configuration = configuration.Value;
            _logger = logger;
            _accessor = accessor;
            _serviceManager = serviceManager;
            _webHostEnviroment = webHostEnviroment;

            SubscribeNotificationQueueMessages();
        }

        private void SubscribeNotificationQueueMessages()
        {
            // Create connection and channel
            var (channel, connection) = _serviceManager.RabbitMQService.CreateChannel(_notificationQueue);

            // Set references for connection and channel
            _connection = connection;
            _channel = channel;

            // Set Event object which listen message from chanel which is sent by producer
            consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, eventArgs) =>
            {
                await ProccesNotificationQueueMessage(eventArgs);
            };

            // Read the message
            _channel.BasicConsume(queue: _notificationQueue, autoAck: true, consumer: consumer);
        }
        
        private async Task ProccesNotificationQueueMessage(BasicDeliverEventArgs eventArgs)
        {
            QueueNotificationActionModel? notificationMessage;

            var body = eventArgs.Body.ToArray();
            var encodedBody = Encoding.UTF8.GetString(body);
            notificationMessage = JsonConvert.DeserializeObject<QueueNotificationActionModel>(encodedBody);

            if (notificationMessage is not null)
            {
                using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(notificationMessage))))
                {
                    _logger.LogInformation("Queue notification message proccess started");
                }

                // Check if email needs to be send
                if (notificationMessage.Email is not null)
                    await SendEmail(notificationMessage.Email);
                
                // Check if sms needs to be send
                if (notificationMessage.Sms is not null)
                    await _serviceManager.SmsService.SendSMS(notificationMessage.Sms.PhoneNumber, notificationMessage.Sms.Message);

                // Check if push notification needs to be send
                if (notificationMessage.FcmPushNotification is not null)
                    await _serviceManager.FcmNotificationService.SendNotification(notificationMessage.FcmPushNotification);              
            }
        }

        private async Task SendEmail(EmailRabbitMqMessage email)
        {
            List<EmailAttachmentModel> attachments = null;

            try
            {
                _logger.LogInformation($"Sending email proccess started");

                string message = GetEmailMessage(email);

                // Get email content and add to logger
                string emailContent = email.Template.HasValue ? email.Template.Value.ToString() : email.Message ?? "Error no message";

                // Check if attachment need's to be send
                if (email.UrlAttachments.Any())
                {
                    foreach (var url in email.UrlAttachments)
                    {
                        attachments = new();
                        attachments.Add(await GetAttachmentFromUrlAsync(url));
                    }
                }

                // Send email
                await _serviceManager.EmailService.SendMail(email.SendTo, message, email.Subject, attachments);
                _logger.LogInformation($"Email successfully sent to {email.SendTo} with content {emailContent}");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while sending email {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// If email template it's not null return message with template; otherwise return just a message
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private string GetEmailMessage(EmailRabbitMqMessage email)
        {
            if (email.Template is not null)
            {
                return email.Template switch
                {
                    EmailTemplateEnum.PasswordReset => EmailTemplate.PasswordReset(_webHostEnviroment, email.User, _configuration),
                    _ => "",
                };
            }
            return email.Message ?? "";
        }

        private async Task<EmailAttachmentModel> GetAttachmentFromUrlAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                var contentType = response.Content.Headers.ContentType.ToString();
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = Path.GetFileName(url);
                var base64 = Convert.ToBase64String(bytes);

                return new EmailAttachmentModel
                {
                    Type = contentType,
                    FileName = fileName,
                    Content = base64
                };
            }
        }
    }
}
