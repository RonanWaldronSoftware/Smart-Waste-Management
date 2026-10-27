using SmartWaste.Service;
using Microsoft.AspNetCore.Mvc;
using SmartWaste.Model.DataTransferObjects;
using Newtonsoft.Json;
using SmartWaste.Model.Helpers.RabbitMQ;
using SmartWaste.Model.Constants;

namespace SmartWaste.Handler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SigfoxController : ControllerBase
    {
        private readonly ILogger<SigfoxController> _logger;
        private readonly IServiceManager _serviceManager;

        public SigfoxController(ILogger<SigfoxController> logger, IServiceManager serviceManager)
        {
            _logger = logger;
            _serviceManager = serviceManager;
        }

        [HttpPost("postReading")]
        public async Task<IActionResult> PostReading([FromQuery] string id, [FromBody] SigfoxBody sigfoxBody)
        {
            try
            {
                using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(sigfoxBody))))
                {
                    _logger.LogInformation($"Sigfox post reading for {id}");
                }

                // Create unit queue model
                var unitQueueModel = new QueueUnitDataActionModel
                {
                    SigFoxData = new SigFoxData
                    {
                        SerialNumber = id,
                        SigfoxBody = sigfoxBody
                    }
                };

                _serviceManager.RabbitMQService.SendMessage(unitQueueModel, GlobalConstants.UnitQueue);

                // Create response
                string responseBody = System.Text.Json.JsonSerializer.Serialize(new
                {
                    id,
                    downlinkData = sigfoxBody.Data.Trim()
                });

                return Ok(responseBody);
            }
            catch (Exception ex)
            {
                using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(ex.Message))))
                {
                    _logger.LogError($"An error occurred while processing Sigfox data for ID: {id}");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
    }
}
