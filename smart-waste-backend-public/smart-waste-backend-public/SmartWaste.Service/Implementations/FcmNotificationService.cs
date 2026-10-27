using SmartWaste.Model.Configuration;
using SmartWaste.Model.Helpers.FcmNotification;
using SmartWaste.Service.Contracts;
using Microsoft.Extensions.Options;
using Google.Apis.Auth.OAuth2;
using Google.Apis.FirebaseCloudMessaging.v1;
using Google.Apis.FirebaseCloudMessaging.v1.Data;
using Google.Apis.Services;

namespace SmartWaste.Service.Implementations
{
    public class FcmNotificationService : IFcmNotificationService
    {
        private readonly FirebaseCloudMessagingService _fcmService;
        private readonly IOptions<Configuration> _configuration;

        public FcmNotificationService(IOptions<Configuration> configuration)
        {
            // Save configuration
            _configuration = configuration;
            
            // Create google credentials
            GoogleCredential credential;
            using (var stream = new FileStream(configuration.Value.FcmNotificationConfiguration.JsonPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(configuration.Value.FcmNotificationConfiguration.BaseUrl);
            }

            // Initialize firebase cloud service
            _fcmService = new FirebaseCloudMessagingService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SmartWaste"
            });
        }

        public async Task<string> SendNotification(FcmPushNotificationModel fcmNotificationModel)
        {
            // Create message for sending 
            var message = new Message
            {
                Token = fcmNotificationModel.DeviceId,
                Notification = new Notification
                {
                    Title = fcmNotificationModel.Title,
                    Body = fcmNotificationModel.Body
                }
            };

            // Add data about project
            var request = _fcmService.Projects.Messages.Send(new SendMessageRequest { Message = message }, $"projects/{_configuration.Value.FcmNotificationConfiguration.ProjectId}");

            // Try to send message
            try
            {
                var response = await request.ExecuteAsync();
                return response.Name;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

