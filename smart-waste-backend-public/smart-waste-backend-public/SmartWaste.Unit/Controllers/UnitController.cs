using AutoMapper;
using SmartWaste.Model.DataTransferObjects;
using SmartWaste.Model.DataTransferObjects.Unit;
using SmartWaste.Repository;
using SmartWaste.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using SmartWaste.Model.Extensions;
using Newtonsoft.Json;
using SmartWaste.Model.Error;
using SmartWaste.Model.JsonModels.Unit;
using SmartWaste.Model.Core;
using SmartWaste.Model.Constants;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Helpers;
using SmartWaste.Model.Helpers.Superfy;
using SmartWaste.Model.Resources;

namespace SmartWaste.Unit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UnitController : ControllerBase
    {
        private readonly ILogger<UnitController> _logger;
        private readonly IServiceManager _serviceManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly LocalizerService _localizer;

        public UnitController(ILogger<UnitController> logger, IServiceManager serviceManager,
            IRepositoryManager repositoryManager, IMapper mapper,
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
        
        [HttpPost("GetUnits")]
        [SwaggerOperation(summary: "Get units", description: "Get all units for that user and his parents")]
        [SwaggerResponse(200, "Get units", typeof(List<UnitTableDto>))]
        public async Task<IActionResult> GetUnits(GetUnitsInput input)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(input))))
            {
                _logger.LogInformation($"Get units requested by {loggedInUserId}");
            }

            // Get possible parents
            var possibleParentsIds =
                await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId,
                    PossibleParentTypeEnum.Unit);

            // Get units from database
            var unitTableResponse = await _repositoryManager.Unit.GetUnits(input, possibleParentsIds, false);

            // Get all alert types
            var allAlertTypes =
                await _repositoryManager.AlertType.GetAlertTypes(AlertTypeCategoryEnum.Warning, false);

            var responseUnits = new List<UnitTableDto>();

            // Get language
            Language language = await _repositoryManager.Language.GetLanguage(languageId, false);

            // Get alert types for last unit reading
            foreach (var unit in unitTableResponse.Responses)
            {
                unit.ContentType.Name = _localizer.GetLocalString(unit.ContentType.TranslationKey, language.Code);

                // Handle Superfy units - get latest telemetry and simulate LatestUnitReadings
                if ((int)DeviceInputTypeEnum.Api == unit.DeviceInputTypeId && !string.IsNullOrEmpty(unit.SuperfyGuid))
                {
                    var superfyTelemetryLatest =
                        await _serviceManager.SuperfyService.GetDeviceTelemetryLatest(unit.SuperfyGuid);

                    if (superfyTelemetryLatest != null)
                    {
                        // Map telemetry to unit reading
                        var latestUnitReading =
                            SuperfyMapper.MapTelemetryToLatestUnitReading(superfyTelemetryLatest, unit);

                        // Create a list to simulate LatestUnitReadings for consistent processing
                        unit.LatestUnitReadings = new List<LatestUnitReading> { latestUnitReading };
                    }
                }

                var unitDto = _mapper.Map<UnitTableDto>(unit);

                if (unit.LatestUnitReadings.Any())
                {
                    // Get lastUnitReading alert types
                    var readingAlerts = unit.LatestUnitReadings.First().GetAlertTypes(allAlertTypes);

                    foreach (var item in readingAlerts)
                    {
                        item.Name = _localizer.GetLocalString(item.TranslationKey, language.Code);
                    }

                    // Convert to dtos
                    var alertTypeDtos = readingAlerts.Select(_mapper.Map<ShortAlertTypeDto>).ToList();

                    unitDto.AlertTypes = alertTypeDtos;

                    // Set fill level
                    unitDto.FillLevelPercentage = unit.ComputeUnitFillLevelPercentages(unit.LatestUnitReadings.First())
                        .FillLevelPercentage;
                }

                responseUnits.Add(unitDto);
            }

            // Create response
            var response = new PaginatedResponse<UnitTableDto>
            {
                Items = responseUnits,
                Count = unitTableResponse.Options.TotalRecords,
            };

            return Ok(response);
        }

        [HttpGet("{unitId:int}")]
        [SwaggerOperation(summary: "Get unit", description: "Get unit")]
        [SwaggerResponse(200, "Return unit", typeof(UnitDetailsDto))]
        [SwaggerResponse(400, "Can't access to unit")]
        public async Task<IActionResult> GetUnit(int unitId)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            _logger.LogInformation($"Get unit {unitId} requested by {loggedInUserId}");

            // Get language
            Language language = await _repositoryManager.Language.GetLanguage(languageId, false);

            // Get possible parents
            var possibleParentsIds =
                await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId,
                    PossibleParentTypeEnum.Unit);

            // Get unit
            var unit = await _repositoryManager.Unit.GetUnit(unitId, false, true);

            // Check if unit is not null and if logged user can access to selected unit
            if (unit is not null && possibleParentsIds.Any(id => id == unit.UserId))
            {
                var unitDetails = _mapper.Map<UnitDetailsDto>(unit);

                // Handle Superfy units - simulate LatestUnitReadings first
                if ((int)DeviceInputTypeEnum.Api == unit.DeviceInputTypeId && !string.IsNullOrEmpty(unit.SuperfyGuid))
                {
                    var latestTelemetry =
                        await _serviceManager.SuperfyService.GetDeviceTelemetryLatest(unit.SuperfyGuid);

                    if (latestTelemetry != null)
                    {
                        // Map telemetry to latest unit reading and simulate LatestUnitReadings
                        var latestReading = SuperfyMapper.MapTelemetryToLatestUnitReading(latestTelemetry, unit);
                        unit.LatestUnitReadings = new List<LatestUnitReading> { latestReading };

                        // Map unit to details DTO 
                        unitDetails = _mapper.Map<UnitDetailsDto>(unit);
                        unitDetails.UnitReading = _mapper.Map<UnitReadingDto>(latestReading);
                    }
                }

                else if (unit.LatestUnitReadings.Any())
                {
                    unitDetails = _mapper.Map<UnitDetailsDto>(unit);

                    // Get last reading
                    var lastReading = unit.LatestUnitReadings.First();

                    unitDetails.FillLevelPercentage = unit
                        .ComputeUnitFillLevelPercentages(unit.LatestUnitReadings.First()).FillLevelPercentage;
                    unitDetails.UnitReading = _mapper.Map<UnitReadingDto>(lastReading);

                    // Get all alert types
                    var allAlertTypes =
                        await _repositoryManager.AlertType.GetAlertTypes(AlertTypeCategoryEnum.Warning,
                            false);

                    // Get lastUnitReading alert types
                    var readingAlerts = lastReading.GetAlertTypes(allAlertTypes);

                    foreach (var item in readingAlerts)
                    {
                        item.Name = _localizer.GetLocalString(item.TranslationKey, language.Code);
                    }

                    // Convert to dtos
                    var alertTypeDtos = readingAlerts.Select(_mapper.Map<ShortAlertTypeDto>).ToList();

                    unitDetails.AlertTypes = alertTypeDtos;
                }

                // Translate 
                unitDetails.UnitType.Name = _localizer.GetLocalString(unit.UnitType.TranslationKey, language.Code);
                unitDetails.ContentType.Name =
                    _localizer.GetLocalString(unit.ContentType.TranslationKey, language.Code);
                return Ok(unitDetails);
            }

            return BadRequest(new HttpError(_localizer.GetLocalString("getUnitErrorResponse", language.Code)));
        }

        [HttpPost]
        [Authorize(Policy = "AdminDistrubCorporateCustomerPolicy")]
        [SwaggerOperation(summary: "Create unit", description: "Create unit")]
        [SwaggerResponse(200)]
        [SwaggerResponse(400, "User can't create unit")]
        public async Task<IActionResult> CreateUnit(CreateUnitInput input)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(input))))
            {
                _logger.LogInformation($"Create unit by user {loggedInUserId}");
            }

            // Get language
            Language language = await _repositoryManager.Language.GetLanguage(languageId, false);

            // Check if any serial number already exist
            var existingSerialNumbers =
                await _repositoryManager.Unit.CheckIfSerialNumbersExist(input.SerialNumbers);

            if (existingSerialNumbers)
                return BadRequest(
                    new HttpError(_localizer.GetLocalString("serialNumberExistErrorResponse", language.Code)));

            // Get logged user
            var loggedUser = await _repositoryManager.User.GetUser(loggedInUserId, false);

            // Get possible parents
            var possibleParentsIds =
                await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId,
                    PossibleParentTypeEnum.Unit);

            if (loggedUser is not null && possibleParentsIds.Any(id => id == input.UserId))
            {
                // Get unit readings without unitId for this serial numbers
                var unitReadingsWithoutUnit =
                    await _repositoryManager.UnitReading.GetUnassignedReadings(input.SerialNumbers);
                
                SuperfyDevicesResponse? devices = null;

                // Check if it's superfy unit and fetch devices only once
                if (input.DeviceInputTypeId == (int)DeviceInputTypeEnum.Api)
                {
                    var request = new SuperfyCompanyDeviceListRequest()
                    {
                        PageSize = 1000
                    };
                    
                    devices = await _serviceManager.SuperfyService.GetCompanyDeviceListAsync(request);
                }
                
                foreach (var serialNumber in input.SerialNumbers)
                {
                    // Create instance of unit and set default values
                    var newUnit = new Model.Core.Unit
                    {
                        StatusId = DatabaseConstants.UnitStatus.Active.Id,
                        SerialNumber = serialNumber,
                        CreateUserId = loggedInUserId,
                    };

                    // Get device by serial number if devices were fetched
                    if (devices is not null)
                    {
                        var selectedDevice = devices.Items?.FirstOrDefault(i => i.Serial.Equals(serialNumber));
                        newUnit.SuperfyGuid = selectedDevice?.Id;
                    }
                    
                    // Map input to new unit and create unit
                    _mapper.Map(input, newUnit);
                    await _repositoryManager.Unit.CreateUnitAsync(newUnit);

                    // Get unassigned readings for that serial number
                    var unitReadings = unitReadingsWithoutUnit.Where(ur => ur.SerialNumber.Equals(serialNumber));

                    // Set last reading with firmware
                    newUnit.FirmWare = unitReadings.Where(ur => ur.Firmware != null).LastOrDefault()?.Firmware;

                    foreach (var unitReading in unitReadings)
                    {
                        unitReading.UnitId = newUnit.Id;
                        unitReading.FullnessStatusId =
                            newUnit.ComputeUnitFillLevelPercentages(unitReading).FillnessStatusId;
                    }
                }

                await _repositoryManager.SaveAsync();

                return Ok();
            }

            return BadRequest(new HttpError(_localizer.GetLocalString("createUnitErrorResponse", language.Code)));
        }

        [HttpPut]
        [Authorize(Policy = "AdminDistrubCorporateCustomerPolicy")]
        [SwaggerOperation(summary: "Update unit", description: "Update unit")]
        [SwaggerResponse(200)]
        [SwaggerResponse(400, "User can't update that unit")]
        public async Task<IActionResult> UpdateUnit(UpdateUnitInput input)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(input))))
            {
                _logger.LogInformation($"Update unit by user {loggedInUserId}");
            }

            // Get possible parents
            var possibleParentsIds =
                await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId,
                    PossibleParentTypeEnum.Unit);

            var unit = await _repositoryManager.Unit.GetUnit(input.Id, true, false);

            // Check if logged user can update that unit
            if (!possibleParentsIds.Any(id => id == unit.UserId))
            {
                Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
                return BadRequest(
                    new HttpError(_localizer.GetLocalString("updateUnitErrorResponse", language.Code)));
            }

            // Check if the new serial number is different and not null
            if (input.SerialNumber is not null && unit.SerialNumber != input.SerialNumber)
            {
                bool serialNumberExists = await _repositoryManager.Unit.IsSerialNumberExist(input.SerialNumber);
                if (serialNumberExists)
                {
                    Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
                    return BadRequest(
                        new HttpError(_localizer.GetLocalString("serialNumberExistErrorResponse", language.Code)));
                }
            }

            // Set update user
            unit.UpdateUserId = loggedInUserId;

            // Merge and save changes
            _mapper.Map(input, unit);
            await _repositoryManager.SaveAsync();

            return Ok();
        }

        [HttpPut("SetStatus")]
        [SwaggerOperation(summary: "Set unit status",
            description: "Set unit status. Toggle status to active/inactive")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404, "Unit not found")]
        public async Task<IActionResult> SetUnitStatus(int unitId)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            // Get unit
            var unit = await _repositoryManager.Unit.GetUnit(unitId, true, false);

            // Get possible parents
            var possibleParentsIds =
                await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId,
                    PossibleParentTypeEnum.Unit);

            if (unit is not null && possibleParentsIds.Any(id => id == unit.UserId))
            {
                // Change status
                unit.StatusId = unit.StatusId == DatabaseConstants.UnitStatus.Active.Id
                    ? DatabaseConstants.UnitStatus.Inactive.Id
                    : DatabaseConstants.UnitStatus.Active.Id;

                await _repositoryManager.SaveAsync();

                _logger.LogInformation($"Unit {unitId} status change to {unit.StatusId} by user {loggedInUserId}");
                return Ok();
            }

            // Get language
            Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
            return NotFound(
                new HttpError(_localizer.GetLocalString("statusUpdateUnitErrorResponse", language.Code)));
        }

        [HttpDelete]
        [SwaggerOperation(summary: "Delete unit",
            description: "Delete unit only if that unit is not on some route")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> DeleteUnit(int unitId)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            _logger.LogInformation($"Delete unit {unitId} by {loggedInUserId}");

            // Get possible parents
            var possibleParentsIds =
                await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId,
                    PossibleParentTypeEnum.Unit);

            // Get unit
            var unit = await _repositoryManager.Unit.GetUnit(unitId);

            // Check if unit is not null, exist in possibleParents and there is no routes for that unit
            if (unit is not null && possibleParentsIds.Any(id => id == unit.UserId) && !unit.RouteUnits.Any())
            {
                unit.Deleted = true;
                await _repositoryManager.SaveAsync();

                return Ok();
            }

            Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
            return BadRequest(new HttpError(_localizer.GetLocalString("deleteUnitErrorResponse", language.Code)));
        }

        [HttpGet("CheckSerialNumber")]
        [SwaggerOperation(summary: "Check serial number", description: "Check if that unit serial number exist")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404)]
        public async Task<IActionResult> CheckSerialNumber(string serialNumber)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            _logger.LogInformation(
                $"Check if serial number already exist {serialNumber} requested by {loggedInUserId}");

            bool isExist = await _repositoryManager.Unit.IsSerialNumberExist(serialNumber);

            if (!isExist)
            {
                Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
                return NotFound(
                    new HttpError(_localizer.GetLocalString("serialNumberCheckUnitErrorResponse", language.Code)));
            }

            return Ok();
        }

        [HttpGet("GetUserUnits")]
        [SwaggerOperation(summary: "Get units", description: "Get units for selected user")]
        [SwaggerResponse(200, "Return user units", typeof(List<ShortUnitDto>))]
        [SwaggerResponse(400, "User can't access to units")]
        public async Task<IActionResult> GetUserUnits(int userId)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            _logger.LogInformation($"Requested get units from {loggedInUserId} for user {userId}");

            // Get possible parents
            var possibleParentsIds =
                await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId,
                    PossibleParentTypeEnum.Unit);

            // Check if logged user can access for unit of selected user (userId)
            if (!possibleParentsIds.Contains(userId))
            {
                Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
                return NotFound(
                    new HttpError(_localizer.GetLocalString("getUserUnitsErrorResponse", language.Code)));
            }

            var units = await _repositoryManager.Unit.GetUnitsForCustomer(userId, false);

            return Ok(_mapper.Map<List<ShortUnitDto>>(units));
        }

        [HttpGet("Readings")]
        [SwaggerOperation(summary: "Get unit readings", description: "Get unit history readings")]
        [SwaggerResponse(200, "Return unit readings", typeof(PaginatedResponse<ShortUnitReadingDto>))]
        public async Task<IActionResult> GetUnitReadings([FromQuery] UnitReadingDatatableOptions input)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(input))))
            {
                _logger.LogInformation($"Get unit readings requested by user {loggedInUserId}");
            }

            // Get possible parents
            var possibleParentsIds =
                await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId,
                    PossibleParentTypeEnum.Unit);

            // Get unit 
            var unit = await _repositoryManager.Unit.GetUnit(input.UnitId, false, false);

            // Check if user can access to those unit readings
            if (unit is not null && possibleParentsIds.Any(id => id == unit.UserId))
            {
                var unitReadingsTableResponse = new TableResponse<UnitReading>();

                if ((int)DeviceInputTypeEnum.Api == unit.DeviceInputTypeId)
                {
                    var historyRequest = new DeviceTelemetryHistoryRequest()
                    {
                        DeviceId = unit.SuperfyGuid ?? "",
                        PageSize = input.PerPage
                    };

                    var superfyHistory =
                        await _serviceManager.SuperfyService.GetDeviceTelemetryHistory(historyRequest);

                    // Map Superfy telemetry history to UnitReadings
                    var unitReadings = SuperfyMapper.MapTelemetryHistoryToUnitReadings(superfyHistory, unit);

                    unitReadingsTableResponse.Responses = unitReadings;
                    unitReadingsTableResponse.Options = new DatatableOptions
                    {
                        TotalRecords = unitReadings.Count,
                    };
                }
                else
                {
                    // Get unit readings from db
                    unitReadingsTableResponse = await _repositoryManager.UnitReading.GetUnitReadings(input, false);
                }

                // Get all alert types
                var allAlertTypes =
                    await _repositoryManager.AlertType.GetAlertTypes(AlertTypeCategoryEnum.Warning, false);

                // Create response
                var response = new UnitReadingPagintetedResponse<ShortUnitReadingDto>
                {
                    Items = [],
                    Count = unitReadingsTableResponse.Options.TotalRecords,
                    Location = unit.Location ?? "",
                    SerialNumber = unit.SerialNumber ?? ""
                };

                foreach (var unitReading in unitReadingsTableResponse.Responses)
                {
                    // Get lastUnitReading alert types
                    var readingAlerts = unitReading.GetAlertTypes(allAlertTypes);
                    // Convert to dtos
                    var alertTypeDtos = readingAlerts.Select(_mapper.Map<ShortAlertTypeDto>).ToList();

                    // Map and add unit reading with alerts to response
                    var item = _mapper.Map<ShortUnitReadingDto>(unitReading);
                    item.AlertTypes.AddRange(alertTypeDtos.Where(a =>
                        a.Id != DatabaseConstants.AlertType.DeviceNotReporting.Id));
                    response.Items.Add(item);
                }

                return Ok(response);
            }

            Language language = await _repositoryManager.Language.GetLanguage(languageId, false);

            return BadRequest(
                new HttpError(_localizer.GetLocalString("unitReadingsAccessErrorResponse", language.Code)));
        }

        [HttpPut("InstallUnit")]
        [SwaggerOperation(summary: "Install unit", description: "Install a new unit")]
        [SwaggerResponse(200, "Unit installed")]
        public async Task<IActionResult> InstallUnit([FromBody] UnitInstallationInput input)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            Model.Core.Unit? unit = await _repositoryManager.Unit.GetUnit(input.SerialNumber);

            var loggedInUser = await _repositoryManager.User.GetUser(loggedInUserId);

            // Get users from unit owner to technician parent
            var possibleParentsIds =
                await _repositoryManager.User.GetAllBranchUserChildrenIds(unit.UserId.Value,
                    loggedInUser.ParentId.Value);

            if (unit is not null && possibleParentsIds.Any(p => p.Equals(loggedInUserId)))
            {
                // Set unit values
                unit.Latitude = decimal.Parse(input.Latitude);
                unit.Longitude = decimal.Parse(input.Longitude);
                unit.InstallationDate = DateTime.UtcNow;
                unit.Location = input.Location;
                unit.StatusId = DatabaseConstants.UnitStatus.Active.Id;
                
                // Update unit
                await _repositoryManager.SaveAsync();

                // Generate unit image file name
                string fileName = Guid.NewGuid() + $"{input.ImageData.Ext}";

                // Get unit installation image
                UnitImage? unitInstallationImage =
                    await _repositoryManager.UnitImage.GetUnitInstallationImage(unit.Id);

                if (unitInstallationImage is not null)
                {
                    unitInstallationImage.Image = await _serviceManager.FileService.UploadFile(
                        Convert.FromBase64String(input.ImageData.Base64), fileName,
                        GlobalConstants.S3InstallsFolder);
                    await _repositoryManager.UnitImage.UpdateUnitImage(unitInstallationImage);
                }
                else
                {
                    // Generate new unit image model
                    UnitImage newUnitImage = new UnitImage
                    {
                        UnitId = unit.Id,
                        UnitCategoryId = DatabaseConstants.UnitCategory.Install.Id,
                        Image = await _serviceManager.FileService.UploadFile(
                            Convert.FromBase64String(input.ImageData.Base64), fileName,
                            GlobalConstants.S3InstallsFolder)
                    };

                    // Create unit image
                    await _repositoryManager.UnitImage.CreateUnitImage(newUnitImage);
                }

                return Ok();
            }

            Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
            return BadRequest(new HttpError(_localizer.GetLocalString("installUnitErrorResponse", language.Code)));
        }

        [HttpPut("LockUnit")]
        [SwaggerOperation(summary: "Lock unit", description: "Lock unit")]
        public async Task<IActionResult> LockUnit(BinLockInput input)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(input))))
            {
                _logger.LogInformation($"Lock unit by user {loggedInUserId}");
            }

            // Get possible parents
            var possibleParentsIds =
                await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId,
                    Model.Enums.PossibleParentTypeEnum.Unit);

            // Get unit 
            var unit = await _repositoryManager.Unit.GetUnit(input.UnitId, true, false);

            // Check if user can access to those unit readings
            if (unit is not null && possibleParentsIds.Any(id => id == unit.UserId))
            {
                await _repositoryManager.Unit.LockBin(unit, input, loggedInUserId);
                return Ok();
            }

            Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
            return BadRequest(new HttpError(_localizer.GetLocalString("lockUnitErrorResponse", language.Code)));
        }
    }
}