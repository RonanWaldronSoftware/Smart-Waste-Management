using AutoMapper;
using SmartWaste.Model.Core;
using SmartWaste.Model.DataTransferObjects;
using SmartWaste.Model.DataTransferObjects.Route;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Error;
using SmartWaste.Model.Helpers;
using SmartWaste.Model.JsonModels.Route;
using SmartWaste.Model.Resources;
using SmartWaste.Repository;
using SmartWaste.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SmartWaste.Unit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RouteController : ControllerBase
    {
        private readonly ILogger<RouteController> _logger;
        private readonly IServiceManager _serviceManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly LocalizerService _localizer;

        public RouteController(ILogger<RouteController> logger, IServiceManager serviceManager, IRepositoryManager repositoryManager, IMapper mapper, 
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

        [HttpGet("{routeId:int}")]
        [SwaggerOperation(summary: "Get route", description: "Get route details")]
        [SwaggerResponse(200, "Return route", typeof(RouteDetailsDto))]
        public async Task<IActionResult> GetRoute(int routeId)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            _logger.LogInformation($"Get route {routeId} requested by {userId}");

            // Get possible parents
            var possibleParentsIds = await _repositoryManager.User.GetPossibleParentsIds(userId, PossibleParentTypeEnum.Route);

            var route = await _repositoryManager.Route.GetRoute(routeId, false);

            // Check if route is not null and if logged user can access to that route
            if (route is not null && possibleParentsIds.Any(id => id == route.UserId))
                return Ok(_mapper.Map<RouteDetailsDto>(route));

            // Get language
            Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
            return BadRequest(new HttpError(_localizer.GetLocalString("canNotGetRouteErrorMessage", language.Code)));
        }

        [HttpGet("Routes")]
        [SwaggerOperation(summary: "Get routes", description: "")]
        [SwaggerResponse(200, "Return routes")]
        public async Task<IActionResult> GetRoutes([FromQuery] DatatableOptions options)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            _logger.LogInformation($"Get routes requested by user {userId}");

            // Get possible parents
            var possibleParentsIds = await _repositoryManager.User.GetPossibleParentsIds(userId, PossibleParentTypeEnum.Route);

            // Get routes from database
            var routeTableResponse = await _repositoryManager.Route.GetRoutes(options, possibleParentsIds, false);

            // Create response
            var response = new PaginatedResponse<RouteDto>
            {
                Items = _mapper.Map<List<RouteDto>>(routeTableResponse.Responses),
                Count = routeTableResponse.Options.TotalRecords,
            };

            return Ok(response);
        }

        [HttpPut]
        [Authorize(Policy = "AdminDistrubCorporateCustomerPolicy")]
        [SwaggerOperation(summary: "Update route", description: "Update route and route units")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> UpdateRoute(RouteUpdateInput input)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(input))))
            {
                _logger.LogInformation($"Update route by user {userId}");
            }

            // Get possible parents
            var possibleParentsIds = await _repositoryManager.User.GetPossibleParentsIds(userId, PossibleParentTypeEnum.Route);

            // Get route
            var route = await _repositoryManager.Route.GetRoute(input.Id, true);

            // Check if that user can update route
            if (route is not null && possibleParentsIds.Any(id => id == route.UserId))
            {
                // Update route and route units
                await _repositoryManager.Route.UpdateRouteAndUnits(route, input, userId);
                return Ok();
            }

            // Get language
            Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
            return BadRequest(new HttpError(_localizer.GetLocalString("canNotUpdateRouteErrorMessage", language.Code)));
        }

        [HttpPost]
        [Authorize(Policy = "AdminDistrubCorporateCustomerPolicy")]
        [SwaggerOperation(summary: "Create route", description: "Create new route with route units")]
        [SwaggerResponse(200)]
        [SwaggerResponse(400, "Unit doesn't exist to that customer")]
        public async Task<IActionResult> CreateRoute(CreateRouteInput model)
        {
            using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(model))))
            {
                _logger.LogInformation("Create route");
            }

            int createUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            // Create route
            var newRoute = await _repositoryManager.Route.CreateRoute(model.Name, model.UserId, createUserId);

            // Get units for that customer
            var customerUnits = await _repositoryManager.Unit.GetUnitsForCustomer(model.UserId, false);

            // Create route units
            foreach (var routeUnit in model.RouteUnits)
            {
                // Check if customer has that unit
                if (!customerUnits.Any(cu => cu.Id == routeUnit.UnitId))
                {
                    // Get language
                    Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
                    return BadRequest(new HttpError(_localizer.GetLocalString("createRouteNoUnitErrorMessage", language.Code)));
                }

                _repositoryManager.RouteUnit.CreateRouteUnit(newRoute.Id, routeUnit.UnitId, routeUnit.RouteIndex.Value);
            }

            await _repositoryManager.SaveAsync();

            return Ok();
        }

        [HttpDelete]
        [Authorize(Policy = "AdminDistrubCorporateCustomerPolicy")]
        [SwaggerOperation(summary: "Delete route", description: "Delete user route and check if user can't delete that route")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> DeleteRoute(int routeId)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            _logger.LogInformation($"Delete route {routeId} by user {userId}");

            // Get possible parents
            var possibleParentsIds = await _repositoryManager.User.GetPossibleParentsIds(userId, PossibleParentTypeEnum.Route);

            // Get route
            var route = await _repositoryManager.Route.GetRoute(routeId);

            // Check it's not null and if logged user can delete that route
            if (route is not null && possibleParentsIds.Any(id => id == route.UserId))
            {
                route.Deleted = true;
                await _repositoryManager.SaveAsync();

                // Delete routeUnit references 
                await _repositoryManager.RouteUnit.DeleteRouteUnits(routeId);

                return Ok();
            }

            // Get language
            Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
            return BadRequest(new HttpError(_localizer.GetLocalString("canNotDeleteRouteErrorMessage", language.Code)));
        }
    }
}
