using SmartWaste.Model.Constants;
using SmartWaste.Model.Helpers.RabbitMQ;
using SmartWaste.Repository;
using SmartWaste.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using AutoMapper;
using SmartWaste.Handler.Service;
using SmartWaste.Model.Core;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Extensions;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Handler.Controllers
{
    public class UnitHandlerController : ControllerBase
    {
        private readonly ILogger<UnitHandlerController> _logger;
        private readonly IServiceManager _serviceManager;
        private readonly IMapper _mapper;
        private const string _unitDataQueue = GlobalConstants.UnitQueue;
        private IDbContextFactory<RepositoryContext> _contextFactory;
        private IRepositoryManager _repositoryManager => new RepositoryManager(_contextFactory.CreateDbContext());

        // Rabbitmq variables
        private IConnection? _connection;
        private IModel? _channel;
        private EventingBasicConsumer _consumer;

        // A dictionary to keep track of failure counts for each message
        private readonly Dictionary<ulong, int> _failureCounts = new();

        public UnitHandlerController(IServiceManager serviceManager, ILogger<UnitHandlerController> logger, IDbContextFactory<RepositoryContext> contextFactory,
            IMapper mapper)
        {
            _serviceManager = serviceManager;
            _contextFactory = contextFactory;
            _logger = logger;
            _mapper = mapper;

            SubscribeUnitDataQueueMessage();
        }

        private void SubscribeUnitDataQueueMessage()
        {
            // Create connection and channel
            var (channel, connection) = _serviceManager.RabbitMQService.CreateChannel(_unitDataQueue);

            // Set references for connection and channel
            _connection = connection;
            _channel = channel;

            // Limit the number of unacknowledged messages that RabbitMQ will deliver
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);

            // Set Event object which listen message from chanel which is sent by producer
            _consumer = new EventingBasicConsumer(_channel);

            _consumer.Received += async (model, eventArgs) =>
            {
                try
                {
                    await ProccesUnitDataQueueMessage(eventArgs);
                    _channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);

                    // If processing succeeds, remove the message from the failure count dictionary
                    _failureCounts.Remove(eventArgs.DeliveryTag);
                }
                catch (Exception ex)
                {
                    // Increment the failure count for this message
                    if (_failureCounts.ContainsKey(eventArgs.DeliveryTag))
                        _failureCounts[eventArgs.DeliveryTag]++;
                    else
                        _failureCounts[eventArgs.DeliveryTag] = 1;

                    _logger.LogError(ex, $"Error handling message from queue - {_failureCounts[eventArgs.DeliveryTag]}");

                    // Check if the failure count has reached 10
                    if (_failureCounts[eventArgs.DeliveryTag] >= 10)
                    {
                        _logger.LogError("Message dropped after 10 failed attempts.");
                        // Drop the message by not requeuing it
                        _channel.BasicNack(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: false);

                        // Optionally, remove the message from the failure count dictionary
                        _failureCounts.Remove(eventArgs.DeliveryTag);
                    }
                    // Nack the message and requeue it
                    else
                        _channel.BasicNack(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: true);
                }
            };

            // Read the message
            _channel.BasicConsume(queue: _unitDataQueue, autoAck: false, consumer: _consumer);
        }

        /// <summary>
        /// Proccess unit data from rabbitmq
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        private async Task ProccesUnitDataQueueMessage(BasicDeliverEventArgs eventArgs)
        {
            QueueUnitDataActionModel? unitDataMessage;

            var body = eventArgs.Body.ToArray();
            var encodedBody = Encoding.UTF8.GetString(body);

            unitDataMessage = JsonConvert.DeserializeObject<QueueUnitDataActionModel>(encodedBody);

            if (unitDataMessage is not null)
            {
                // Get all possible alert types from db
                var allAlerts = await _repositoryManager.AlertType.GetAlertTypes(AlertTypeCategoryEnum.Warning, false);

                if (unitDataMessage.DataSensor is not null)
                {
                    // Get binary unit data
                    var unitBinaryData = unitDataMessage.DataSensor.Data;

                    if (unitBinaryData is not null)
                    {
                        using (_logger.BeginScope(("full_message", BitConverter.ToString(unitBinaryData))))
                        {
                            _logger.LogInformation("Queue unit data message start proccessing");
                        }

                        var unitRead = await UnitStaticService.ProcessSmartWasteData(unitDataMessage.DataSensor, _repositoryManager, _logger, _mapper, _serviceManager);

                        // Check if unit reading si different then null
                        if (unitRead != null)
                        {
                            // Get unit reading from database with related users and unit
                            var dbUnitReading = await _repositoryManager.UnitReading.GetUnitReading(unitRead.Id);

                            // Send alert only if unit exist
                            if (dbUnitReading?.Unit != null)
                            {
                                // Get alerts from reading that needs to be send
                                var alertTypeForSend = dbUnitReading.GetAlertTypes(allAlerts);
                                await _serviceManager.AlertService.SendAlert(dbUnitReading.Unit!.UserId!.Value, alertTypeForSend, dbUnitReading.Unit);
                            }
                        }
                    }
                }
                else if (unitDataMessage.SigFoxData is not null)
                {
                    using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(unitDataMessage.SigFoxData))))
                    {
                        _logger.LogInformation($"Start processing sigfox data for id: {unitDataMessage.SigFoxData.SerialNumber}");
                    }

                    var unitRead = await UnitStaticService.ProcessSigfoxData(unitDataMessage.SigFoxData, _repositoryManager, _logger, _mapper, _serviceManager);

                    // Get unit reading from database with related users and unit
                    var dbUnitReading = await _repositoryManager.UnitReading.GetUnitReading(unitRead.Id);

                    if (dbUnitReading is not null && dbUnitReading.Unit is not null)
                    {
                        // Get alerts from reading that needs to be send
                        var alertTypeForSend = dbUnitReading.GetAlertTypes(allAlerts);

                        // Send alert
                        await _serviceManager.AlertService.SendAlert(dbUnitReading.Unit.UserId.Value, alertTypeForSend, dbUnitReading.Unit);
                    }
                }
            }
        }
    }
}
