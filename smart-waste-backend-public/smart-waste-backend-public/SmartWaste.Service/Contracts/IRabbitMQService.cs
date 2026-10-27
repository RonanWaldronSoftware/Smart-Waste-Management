using RabbitMQ.Client;

namespace SmartWaste.Service.Contracts
{
    public interface IRabbitMQService
    {
        /// <summary>
        /// Send message to rabbit mq
        /// </summary>
        /// <param name="data"></param>
        /// <param name="queue"></param>
        void SendMessage(object data, string queue);
        /// <summary>
        /// Read message from rabbit mq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <returns></returns>
        T? GetMessage<T>(string queue);
        /// <summary>
        /// Create rabbit mq connection and channel
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        (IModel channel, IConnection connection) CreateChannel(string queue);
    }
}
