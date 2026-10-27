using System.Net.Sockets;
using System.Net;
using System.Text;
using SmartWaste.Model.Configuration;
using Microsoft.Extensions.Options;
using SmartWaste.Model.Constants;
using SmartWaste.Model.Helpers.RabbitMQ;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SmartWaste.Model.Core;
using Microsoft.EntityFrameworkCore;
using SmartWaste.Model.Extensions;

namespace SmartWaste.Tcp.Services
{
    public class TcpListenerService : BackgroundService
    {
        private readonly ILogger<TcpListenerService> _logger;
        private readonly TcpConfiguration _tcpConfig;
        private readonly List<TcpListener> _listeners = [];
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IDbContextFactory<RepositoryContext> _contextFactory;

        public TcpListenerService(ILogger<TcpListenerService> logger, IOptions<Configuration> config, IDbContextFactory<RepositoryContext> contextFactory)
        {
            _logger = logger;
            _tcpConfig = config.Value.TcpConfiguration;
            _contextFactory = contextFactory;
            
            // Get rabbit mq configuration
            var rabbitMqConfig = config.Value.RabbitMQConfiguration;

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
            
            // Ensure the queue is declared before publishing
            _channel.QueueDeclare(GlobalConstants.UnitQueue, exclusive: false, durable: true, autoDelete: false);

            // Add ports
            foreach (var listener in _tcpConfig.Ports.Select(port => new TcpListener(IPAddress.Any, port)))
            {
                _listeners.Add(listener);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var listener in _listeners)
            {
                listener.Start();
            }

            var tasks = _listeners.Select(listener => Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
                        _ = Task.Run(() => HandleClientAsync(client, stoppingToken, port));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error accepting client.");
                    }
                }
            }));

            await Task.WhenAll(tasks);

            foreach (var listener in _listeners)
            {
                listener.Stop();
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken stoppingToken, int port)
        {
            using var stream = client.GetStream();
            using var memoryStream = new MemoryStream();

            try
            {
                var buffer = new byte[1024];
                while (!stoppingToken.IsCancellationRequested)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, stoppingToken);
                    if (bytesRead == 0)
                        break;

                    // Process the data immediately after reading it
                    byte[] result = new byte[bytesRead];
                    Array.Copy(buffer, 0, result, 0, bytesRead);

                    _logger.LogInformation($"Bytes received from port {port}: {BitConverter.ToString(result, 0, bytesRead)}");

                    // Determine sensor type
                    var sensorType = port == 48999 ? SensorEnum.SmartWaste : SensorEnum.Tekelek;

                    // Create unit queue model and send it to the queue
                    var unitQueueModel = new QueueUnitDataActionModel
                    {
                        DataSensor = new DataSensorModel
                        {
                            Data = result,
                            Sensor = sensorType
                        }
                    };

                    SendDataIntoQue(unitQueueModel, GlobalConstants.UnitQueue);

                    // Send downlink message for SmartWaste only when a lock command is pending
                    if (sensorType == SensorEnum.SmartWaste)
                    {
                        try
                        {
                            string? serialNumber = ExtractSerialNumber(result, sensorType);

                            if (!string.IsNullOrEmpty(serialNumber))
                            {
                                var unit = await GetUnitLockStatus(serialNumber);

                                if (unit != null && unit.LockCommandPending)
                                {
                                    byte[] downlinkMessage = BuildDownlinkMessage(unit);

                                    await stream.WriteAsync(downlinkMessage, 0, downlinkMessage.Length, stoppingToken);
                                    await stream.FlushAsync(stoppingToken);

                                    _logger.LogInformation($"Downlink message sent: {BitConverter.ToString(downlinkMessage)}");

                                    // Clear the pending flag
                                    await ClearLockCommandPending(serialNumber);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing downlink message");
                        }
                    }

                    // Break after processing one message (bin waits 10 seconds, then closes connection)
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling client.");
            }
            finally
            {
                client.Close();
                client.Dispose();
                stream.Close();
                stream.Dispose();
                memoryStream.Close();
                memoryStream.Dispose();
            }
        }

        /// <summary>
        /// Extract serial number from uplink message based on message type
        /// </summary>
        /// <param name="data">Uplink data bytes</param>
        /// <param name="sensor">Sensor type (SmartWaste or Tekelek)</param>
        /// <returns>Serial number or null if not found</returns>
        private string? ExtractSerialNumber(byte[] data, SensorEnum sensor)
        {
            try
            {
                if (sensor == SensorEnum.SmartWaste)
                {
                    // For SmartWaste (message types 1 and 5)
                    int messageType = data[0] & 0xff;

                    if (messageType == 1 || messageType == 5)
                    {
                        int idSize = data[12];
                        if (data.Length < (13 + idSize))
                            return null;

                        string serialNumber = string.Concat(Enumerable.Range(13, idSize).Select(i => data[i].ToString("X2"))).TrimStart('0').ToUpper();
                        return string.IsNullOrEmpty(serialNumber) ? null : serialNumber.Trim();
                    }
                }
                else if (sensor == SensorEnum.Tekelek)
                {
                    // For Tekelek, use the ExtractSerialNumber extension method
                    return data.ExtractSerialNumber();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting serial number from uplink data");
                return null;
            }
        }

        /// <summary>
        /// Build 8-byte downlink message for bin lock/unlock command
        /// </summary>
        /// <param name="unit">Unit entity with lock status</param>
        /// <returns>8-byte downlink message</returns>
        private byte[] BuildDownlinkMessage(Unit unit)
        {
            // Build message type 4 (time sync message)
            byte[] downlinkMessage = new byte[8];

            // Byte 0: Message type 4
            downlinkMessage[0] = 0x04;

            // Bytes 1-6: Time fields (set to 0x00, ignored on newer PCB201)
            downlinkMessage[1] = 0x00; // year
            downlinkMessage[2] = 0x00; // month
            downlinkMessage[3] = 0x00; // day
            downlinkMessage[4] = 0x00; // hours
            downlinkMessage[5] = 0x00; // minutes
            downlinkMessage[6] = 0x00; // seconds

            // Byte 7: Lock/Unlock flags
            // Logic: Lock if BinLock = true AND (LockUntil is null OR LockUntil is in future)
            // Otherwise unlock
            bool shouldLock = unit.BinLock && (!unit.LockUntil.HasValue || unit.LockUntil.Value > DateTime.UtcNow);

            if (shouldLock)
            {
                downlinkMessage[7] = 0x01; // Lock the bin
                _logger.LogInformation($"Downlink: LOCK command for unit {unit.SerialNumber}");
            }
            else
            {
                downlinkMessage[7] = 0x02; // Unlock the bin
                _logger.LogInformation($"Downlink: UNLOCK command for unit {unit.SerialNumber}");
            }

            return downlinkMessage;
        }

        /// <summary>
        /// Get unit lock status from database by serial number
        /// </summary>
        /// <param name="serialNumber">Unit serial number</param>
        /// <returns>Unit entity or null if not found</returns>
        private async Task<Unit?> GetUnitLockStatus(string serialNumber)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var unit = await context.Units
                    .Where(u => u.SerialNumber == serialNumber && !u.Deleted)
                    .Select(u => new Unit
                    {
                        Id = u.Id,
                        SerialNumber = u.SerialNumber,
                        BinLock = u.BinLock,
                        LockUntil = u.LockUntil,
                        LockCommandPending = u.LockCommandPending
                    })
                    .FirstOrDefaultAsync();

                return unit;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching unit lock status for serial number: {serialNumber}");
                return null;
            }
        }

        /// <summary>
        /// Clear the LockCommandPending flag after successfully sending a downlink
        /// </summary>
        /// <param name="serialNumber">Unit serial number</param>
        private async Task ClearLockCommandPending(string serialNumber)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var unit = await context.Units
                    .Where(u => u.SerialNumber == serialNumber && !u.Deleted)
                    .FirstOrDefaultAsync();

                if (unit != null)
                {
                    unit.LockCommandPending = false;
                    await context.SaveChangesAsync();
                    _logger.LogInformation($"LockCommandPending cleared for unit {serialNumber}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error clearing LockCommandPending for serial number: {serialNumber}");
            }
        }

        /// <summary>
        /// Send data into rabbit mq
        /// </summary>
        /// <param name="data"></param>
        /// <param name="queue"></param>
        /// <exception cref="Exception"></exception>
        private void SendDataIntoQue(QueueUnitDataActionModel data, string queue)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(data, settings);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            // Set attempt on 5 times
            var attempt = 0;
            const int maxAttempts = 5;
            var success = false;

            while (attempt < maxAttempts && !success)
            {
                try
                {
                    // Check if date are not null
                    if (data.DataSensor != null && data.DataSensor.Data != null) 
                        _logger.LogInformation($"Sending data to que: {BitConverter.ToString(data.DataSensor.Data)}");
                    
                    _channel.BasicPublish(exchange: string.Empty, routingKey: queue, basicProperties: properties, body: body);
                    
                    // Check if date are not null
                    if (data.DataSensor != null && data.DataSensor.Data != null) 
                        _logger.LogInformation($"Data send to que: {BitConverter.ToString(data.DataSensor.Data)}");
                    
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
    }
}