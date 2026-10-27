using SmartWaste.Model.Configuration;
using SmartWaste.Service.Contracts;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace SmartWaste.Service.Implementations
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService(IOptions<Configuration> configuration)
        {
            var rabbitMqConfig = configuration.Value.RabbitMQConfiguration;

            // Create one connection to rabbit mq
            var factory = new ConnectionFactory
            {
                HostName = rabbitMqConfig.Url,
                UserName = rabbitMqConfig.Username,
                Password = rabbitMqConfig.Password,
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                TopologyRecoveryEnabled = true,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                ContinuationTimeout = TimeSpan.FromSeconds(30)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public (IModel channel, IConnection connection) CreateChannel(string queue)
        {
            _channel.QueueDeclare(queue, exclusive: false, durable: true, autoDelete: false);
            return (_channel, _connection);
        }

        public T? GetMessage<T>(string queue)
        {
            try
            {
                _channel.QueueDeclare(queue, exclusive: false, durable: true, autoDelete: false);
                var result = _channel.BasicGet(queue, true);

                if (result != null)
                {
                    var body = result.Body.ToArray();
                    var originalJsonString = Encoding.UTF8.GetString(body);
                    return JsonConvert.DeserializeObject<T>(originalJsonString);
                }
            }
            catch (Exception ex)
            {
                return default;
            }

            return default;
        }

        public void SendMessage(object data, string queue)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(data, settings);
            var body = Encoding.UTF8.GetBytes(json);

            // Ensure the queue is declared before publishing
            _channel.QueueDeclare(queue, exclusive: false, durable: true, autoDelete: false);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            // Set attempt on 5 times
            var attempt = 0;
            const int maxAttempts = 10;
            var success = false;

            while (attempt < maxAttempts && !success)
            {
                try
                {
                    _channel.BasicPublish(exchange: string.Empty, routingKey: queue, basicProperties: properties, body: body);
                    success = true;
                }
                catch (Exception ex)
                {
                    attempt++;
                    if (attempt >= maxAttempts)
                        throw new Exception(ex.Message + " - code exception");
                    
                    // Wait before retrying (exponential backoff)
                    Thread.Sleep(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
                }
            }
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
