using AutoMapper;
using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Model.DataTransferObjects;
using SmartWaste.Model.DataTransferObjects.Damage;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Error;
using SmartWaste.Model.Helpers;
using SmartWaste.Model.JsonModels.Damage;
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
    public class DamageController : ControllerBase
    {
        private readonly ILogger<DamageController> _logger;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IServiceManager _serviceManager;
        private readonly IMapper _mapper;
        private readonly LocalizerService _localizer;

        public DamageController(ILogger<DamageController> logger, IRepositoryManager repositoryManager, IMapper mapper, IServiceManager serviceManager, 
            IHttpContextAccessor accessor, LocalizerService localizer)
        {
            _logger = logger;
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _serviceManager = serviceManager;
            _localizer = localizer;

            // Add ip address and browser in scope
            _logger.BeginScope(new Dictionary<string, object>
            {
                ["ip_address"] = accessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                ["browser"] = accessor.HttpContext.Request.Headers.UserAgent.ToString()
            });
        }

        [HttpGet]
        [SwaggerOperation(summary: "Get damages", description: "Get all damages")]
        [SwaggerResponse(200, "Return damages", typeof(PaginatedResponse<DamageTableDto>))]
        public async Task<IActionResult> GetDamages([FromQuery] DatatableOptions options)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            _logger.LogInformation($"Get damages requested by user {loggedInUserId}");

            // Get possible parents
            var possiblePerentIds = await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId, PossibleParentTypeEnum.Unit);

            var damagesTableResponse = await _repositoryManager.Damage.GetActiveDamages(options, possiblePerentIds, false);

            Language language = await _repositoryManager.Language.GetLanguage(languageId, false);

            // Translate status and type names
            foreach ( var damage in damagesTableResponse.Responses)
            {
                damage.Status.Name = _localizer.GetLocalString(damage.Status.TranslationKey, language.Code);
                damage.DamageType.Name = _localizer.GetLocalString(damage.DamageType.TranslationKey, language.Code);
                damage.Unit.UnitModelType.Name = _localizer.GetLocalString(damage.Unit.UnitModelType.TranslationKey, language.Code);
            }

            // Create response
            var response = new PaginatedResponse<DamageTableDto>
            {
                Items = _mapper.Map<List<DamageTableDto>>(damagesTableResponse.Responses),
                Count = damagesTableResponse.Options.TotalRecords,
            };

            return Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(summary: "Create damage report", description: "Create damage report")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> CreateDamage(CreateDamageInput input)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(input))))
            {
                _logger.LogInformation($"Create damage by user {loggedInUserId}");
            }

            var possiblePerentIds = await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId, PossibleParentTypeEnum.Unit);

            // Get unit
            var unit = await _repositoryManager.Unit.GetUnit(input.SerialNumber, false);

            if (unit is not null && possiblePerentIds.Any(id => id == unit.UserId))
            {
                // Upload image
                string fileName = Guid.NewGuid().ToString() + $"{input.Image.Ext}";
                var uploadedImageUrl = await _serviceManager.FileService.UploadFile(Convert.FromBase64String(input.Image.Base64), fileName, GlobalConstants.S3DamageFolder);

                await _repositoryManager.Damage.CreateDamage(input, unit.Id, uploadedImageUrl, loggedInUserId);

                // Get damage alert and send
                AlertType damageAlert = await _repositoryManager.AlertType.GetAlertType((int)AlertTypeEnum.DamageReport);
                await _serviceManager.AlertService.SendAlert(unit.UserId.Value, new List<AlertType> { damageAlert }, unit);

                return Ok();
            }

            return NotFound(new HttpError("Unit not found"));
        }

        [HttpPut]
        [Authorize(Policy = "AllWithoutDriverPolicy")]
        [SwaggerOperation(summary: "Update damage", description: "Update damage status and add new damage history")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404, "Damage not found")]
        public async Task<IActionResult> UpdateDamage(UpdateDamageInput input)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(input))))
            {
                _logger.LogInformation($"Update damage by user {loggedInUserId}");
            }

            var possibleParents = await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId, Model.Enums.PossibleParentTypeEnum.Damage);

            // Get damage
            var damage = await _repositoryManager.Damage.GetDamage(input.DamageId);

            if (damage is not null && possibleParents.Any(id => id == damage.Unit.UserId))
            {
                // Add new damage history
                var damageHistory = _mapper.Map<DamageHistory>(input);
                damageHistory.UserId = damage.DamageHistories.First().UserId;

                // Set damage status and add history
                damage.StatusId = input.StatusId;
                damage.DamageHistories.Add(damageHistory);

                await _repositoryManager.SaveAsync();

                return Ok();
            }

            Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
            return NotFound(new HttpError(_localizer.GetLocalString("damageNotFoundErrorMessage", language.Code)));
        }

        [HttpGet("UnitDamages")]
        [SwaggerOperation(summary: "Get unit history damages", description: "Get all unit history damages")]
        [SwaggerResponse(200, "Return damages", typeof(List<DamageHistoryDto>))]
        public async Task<IActionResult> GetUnitDamages(int unitId)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            _logger.LogInformation($"Get unit history damages requested by user {loggedInUserId}");

            var possiblePerentIds = await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId, Model.Enums.PossibleParentTypeEnum.Unit);

            // Get unit
            var unit = await _repositoryManager.Unit.GetUnit(unitId, false, false);

            if (unit is not null && possiblePerentIds.Any(id => id == unit.UserId))
                return Ok(_mapper.Map<List<DamageDetailsDto>>(await _repositoryManager.Damage.GetUnitDamages(unitId, false)));

            return Ok(new List<DamageDetailsDto>());
        }
    }
}
