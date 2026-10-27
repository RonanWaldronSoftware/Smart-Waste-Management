using SmartWaste.Account.AppSettings;
using SmartWaste.Model.Configuration;
using SmartWaste.Model.Core;
using SmartWaste.Model.DataTransferObjects;
using SmartWaste.Model.DataTransferObjects.Dashboard;
using SmartWaste.Model.JsonModels.Account;
using SmartWaste.Repository;
using SmartWaste.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SmartWaste.Account.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConfigurationController : ControllerBase
    {
        private readonly ILogger<ConfigurationController> _logger;
        private readonly IOptions<Configuration> _configuration;
        private readonly IServiceManager _serviceManager;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IRepositoryManager _repositoryManager;

        public ConfigurationController(ILogger<ConfigurationController> logger, IOptions<Configuration> configuration,
            IServiceManager serviceManager, IJwtAuthManager jwtAuthManager,
            IRepositoryManager repositoryManager, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _serviceManager = serviceManager;
            _repositoryManager = repositoryManager;
            _jwtAuthManager = jwtAuthManager;
            _configuration = configuration;

            // Add ip address and browser in scope
            _logger.BeginScope(new Dictionary<string, object>
            {
                ["ip_address"] = accessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                ["browser"] = accessor.HttpContext.Request.Headers.UserAgent.ToString()
            });
        }

        [HttpGet]
        [SwaggerOperation(summary: "Static configuration data", description: "All static configuration data from database")]
        [SwaggerResponse(200, "Get web configurations")]
        [SwaggerResponse(400, "Get web configurations failed")]
        public async Task<IActionResult> Configuration()
        {
            // Get user claims
            int languageId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ("LanguageId"))?.Value ?? "1");
            int loggedInUserRole = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.Role).Value);

            // Get configurations
            WebConfigurationResponse config = await _serviceManager.ConfigurationService.GetConfiguration(languageId, loggedInUserRole);

            return Ok(config);
        }

        [HttpGet("NavigationCount")]
        [SwaggerOperation(summary: "Get navigation count", description: "Get navigation count for damage and unit")]
        [SwaggerResponse(200, "Get navigation count", typeof(NavigationCountDto))]
        public async Task<IActionResult> GetNavigationCount()
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var possibleParentsIds = await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId, Model.Enums.PossibleParentTypeEnum.Unit);

            var response = new NavigationCountDto
            {
                Unit = await _repositoryManager.Unit.GetUnitNavigationCount(possibleParentsIds, false),
                Damage = await _repositoryManager.Damage.GetNavigationCount(possibleParentsIds, false),
            };

            return Ok(response);
        }

        [HttpGet("Dashboard")]
        [SwaggerOperation(summary: "Get dashboard", description: "Get dashboard statistic")]
        [SwaggerResponse(200, "Get dashboard statistic", typeof(DashboardDto))]
        public async Task<IActionResult> GetDashboard()
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var possibleParentsIds = await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId, Model.Enums.PossibleParentTypeEnum.Unit);

            var dashboard = await _repositoryManager.Unit.GetDashboardStatistic(possibleParentsIds, false);

            return Ok(dashboard);
        }
    }
}
