using AutoMapper;
using SmartWaste.Model.Core;
using SmartWaste.Model.DataTransferObjects.Unit;
using SmartWaste.Model.Error;
using SmartWaste.Model.JsonModels.Unit;
using SmartWaste.Model.Resources;
using SmartWaste.Repository;
using SmartWaste.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SmartWaste.Unit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlertController : ControllerBase
    {
        private readonly ILogger<UnitController> _logger;
        private readonly IServiceManager _serviceManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly LocalizerService _localizer;

        public AlertController(ILogger<UnitController> logger, IServiceManager serviceManager, IRepositoryManager repositoryManager, IMapper mapper,
            IHttpContextAccessor accessor, LocalizerService localizer)
        {
            _logger = logger;
            _serviceManager = serviceManager;
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _localizer = localizer;

            // Add ip address and browser in scope
            _logger.BeginScope(new Dictionary<string, object>
            {
                ["ip_address"] = accessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                ["browser"] = accessor.HttpContext.Request.Headers.UserAgent.ToString()
            });
        }

        [HttpPost]
        [SwaggerOperation(summary: "Create new user alerts", description: "Create new user alerts on platform")]
        [SwaggerResponse(200, "User alerts created")]
        [SwaggerResponse(400, "User alerts create failed", typeof(HttpError))]
        public async Task<IActionResult> CreateUserAlerts([FromBody] List<UserAlertInput> model)
        {
            // Get claims
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            try
            {
                // Create a list of user alerts
                return (await _serviceManager.UserService.CreateUserAlert(model, loggedInUserId, languageId)).GenerateResponse(this);
            }
            catch (Exception ex)
            {
                Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
                return ex.GenerateBadRequest(_logger, this, $"Create user alerts failed", _localizer.GetLocalString("createUserAlertErrorResponse", language.Code));
            }
        }

        [HttpGet]
        [SwaggerOperation(summary: "Get users with alerts", description: "Get users sorted by role with alerts from platform")]
        [SwaggerResponse(200, "Got users with alerts")]
        [SwaggerResponse(400, "User alerts get failed", typeof(HttpError))]
        public async Task<IActionResult> GetUserAlerts(int userId)
        {
            // Get claims
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            // Get a list of alerts with users
            List<AlertUserDto>? alerts = await _serviceManager.UserService.GetAlertsWithUsers(loggedInUserId, userId, languageId);
            if (alerts is null)
                return BadRequest();

            return Ok(alerts);
        }
    }
}
