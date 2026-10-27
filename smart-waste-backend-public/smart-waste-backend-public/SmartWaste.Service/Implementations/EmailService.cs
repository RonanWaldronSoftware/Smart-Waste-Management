using SmartWaste.Model.Configuration;
using SmartWaste.Model.Helpers.Email;
using SmartWaste.Service.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace SmartWaste.Service.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailConfiguration _emailConfiguration;

        public EmailService(ILogger<EmailService> logger, IOptions<Configuration> configuration)
        {
            _logger = logger;
            _emailConfiguration = configuration.Value.EmailConfiguration;
        }

        public async Task SendMail(string to, string message, string subject, IEnumerable<EmailAttachmentModel>? attachments = null)
        {
            var (request, client) = CreateRequest(to, message, subject, attachments);

            await SendRequest(client, request);
        }

        private (RestRequest request, RestClient client) CreateRequest(string to, string message, string subject, IEnumerable<EmailAttachmentModel>? attachments = null)
        {
            // Create rest client object with authentication and base url
            var options = new RestClientOptions(_emailConfiguration.BaseUrl);
            var client = new RestClient(options);

            RestRequest request = new("", Method.Post);

            // Set data for sending email
            request.AddHeader("Authorization", $"Bearer {_emailConfiguration.ApiKey}");
            request.AddHeader("Content-Type", "application/json");

            // Create sending body
            var model = new EmailSendModel()
            {
                From = new EmailFromModel
                {
                    Email = _emailConfiguration.FromNoReplay,
                    Name = "SmartWaste"
                },
                To = [new EmailToModel { Email = to }],
                Subject = subject,
                Html = message,
                Category = "System message"
            };

            // Check if attachments exist and add to body
            if (attachments != null)
                model.Attachments = attachments.ToList();

            // Add parameters from json
            request.AddParameter("application/json", JsonConvert.SerializeObject(model), ParameterType.RequestBody);

            return (request, client);
        }

        private  async Task SendRequest(RestClient client, RestRequest request)
        {
            var response = client.Execute(request);

            if (!response.IsSuccessStatusCode)
                _logger.LogError($"Error while sending email {response.Content}");

        }
    }
}
