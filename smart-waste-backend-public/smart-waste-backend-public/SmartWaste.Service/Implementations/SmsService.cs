using SmartWaste.Model.Configuration;
using SmartWaste.Model.Extensions;
using SmartWaste.Service.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SmartWaste.Service.Implementations
{
    public class SmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly SmsConfiguration _smsConfiguration;
        private readonly ILogger<SmsService> _logger;

        public SmsService(HttpClient httpClient, IOptions<Configuration> configuration, ILogger<SmsService> logger)
        {
            _httpClient = httpClient;
            _smsConfiguration = configuration.Value.SmsConfiguration;
            _logger = logger;
        }

        public async Task SendSMS(string phoneNumber, string message)
        {
            try
            {
                _logger.LogInformation($"Sending sms to {phoneNumber}");

                string userPwd = $"{_smsConfiguration.UserName}:{_smsConfiguration.Password}";
                string basicAuthorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(userPwd));

                var request = new HttpRequestMessage(HttpMethod.Post, _smsConfiguration.BaseURL);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuthorization);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                phoneNumber = phoneNumber.StripPhoneNo();

                var requestBody = new
                {
                    channels = new[] { "SMS" },
                    destinations = new[] { new { phoneNumber } },
                    transactionId = Guid.NewGuid().ToString(),
                    dlr = false,
                    tag = "SmartWaste alert",
                    sms = new { sender = _smsConfiguration.Sender, text = message }
                };

                request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error while sending sms: {response.StatusCode} {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while sending sms {ex.Message}");
            }
        }
    }
}
