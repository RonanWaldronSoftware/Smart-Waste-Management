using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Enums.Export;
using SmartWaste.Model.Error;
using SmartWaste.Model.JsonModels;
using SmartWaste.Model.Resources;
using SmartWaste.Model.Static;
using SmartWaste.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Handler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExportController : ControllerBase
    {
        private readonly ILogger<ExportController> _logger;
        private IDbContextFactory<RepositoryContext> _contextFactory;
        private readonly LocalizerService _localizer;
        private IRepositoryManager _repositoryManager => new RepositoryManager(_contextFactory.CreateDbContext());

        public ExportController(ILogger<ExportController> logger, IDbContextFactory<RepositoryContext> contextFactory, LocalizerService localizer)
        {
            _logger = logger;
            _contextFactory = contextFactory;
            _localizer = localizer;
        }

        [HttpPost]
        [SwaggerOperation(summary: "Export data", description: "Export users(id-1), units(id-2), and unit readings(id-3) data to PDF(id-1) and CSV(id-2) formats.")]
        [SwaggerResponse(200, "Export data")]
        [SwaggerResponse(400, "Wrong export format", typeof(HttpError))]
        public async Task<IActionResult> ExportData(ExportInput input)
        {
            int loggedUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int loggedInUserRole = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.Role).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(input))))
            {
                _logger.LogInformation($"Export data by user {loggedUserId}");
            }

            if (input.ExportFormatId == (int)ExportFormatEnum.PDF)
                return await ExportData(input, "application/pdf", loggedUserId, loggedInUserRole);
            else if (input.ExportFormatId == (int)ExportFormatEnum.CSV)
                return await ExportData(input, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", loggedUserId, loggedInUserRole);

            // Get language
            Language language = await _repositoryManager.Language.GetLanguage(languageId, false);
            return BadRequest(new HttpError(_localizer.GetLocalString("wrongExportFormatResponse", language.Code)));
        }

        #region private

        /// <summary>
        /// Export data based on content type
        /// </summary>
        /// <param name="input"></param>
        /// <param name="contentType"></param>
        /// <param name="loggedUserId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task<IActionResult> ExportData(ExportInput input, string contentType, int loggedUserId, int loggedInUserRole)
        {
            bool hasPermission = false;
            string fileName = string.Empty;
            byte[] fileContent;
            var possibleParentIds = new List<int>();

            switch ((ExportDataType)input.ExportDataTypeId)
            {
                case ExportDataType.User:
                    var users = await GetDataAsync<User>(input);
                    var isAdmin = loggedInUserRole == DatabaseConstants.Role.Admin.Id;
                    possibleParentIds = await _repositoryManager.User.GetPossibleUserIds(isAdmin, loggedUserId, false);
                    hasPermission = !input.DataIds.Any(id => !possibleParentIds.Contains(id)); // Check if every input id is in possibleParentsIds
                    fileContent = contentType == "application/pdf" ? PdfUtility.GeneratePdf(users) : ExcelUtility.GenerateExcel(users);
                    fileName = "Users report";
                    break;

                case ExportDataType.Unit:
                    var units = await GetDataAsync<Unit>(input);
                    if (units == null || units.Count == 0)
                        return NotFound(new HttpError("No units found for the given filters."));
                    possibleParentIds = await _repositoryManager.User.GetPossibleParentsIds(loggedUserId, PossibleParentTypeEnum.Unit);
                    hasPermission = units.All(unit => unit.UserId.HasValue && possibleParentIds.Contains(unit.UserId.Value));
                    fileContent = contentType == "application/pdf" ? PdfUtility.GeneratePdf(units) : ExcelUtility.GenerateExcel(units);
                    fileName = "Units report";
                    break;

                case ExportDataType.UnitReadings:
                    var unitReadings = await GetDataAsync<UnitReading>(input);
                    if (unitReadings == null || unitReadings.Count == 0)
                        return NotFound(new HttpError("No readings found for the given filters."));
                    var allAlerts = await _repositoryManager.AlertType.GetAlertTypes(AlertTypeCategoryEnum.Warning, false);
                    possibleParentIds = await _repositoryManager.User.GetPossibleParentsIds(loggedUserId, PossibleParentTypeEnum.Unit);
                    hasPermission = unitReadings.All(ur => ur.Unit?.UserId.HasValue == true
                                                           && possibleParentIds.Contains(ur.Unit.UserId.Value));
                    fileContent = contentType == "application/pdf" ? PdfUtility.GeneratePdf(unitReadings, allAlerts, input.TimeZoneOffset) : ExcelUtility.GenerateExcel(unitReadings, allAlerts, input.TimeZoneOffset);
                    fileName = "Unit Readings report";
                    break;

                default:
                    throw new InvalidOperationException("Unsupported export data type.");
            }

            if (hasPermission)
            {
                return new FileContentResult(fileContent, contentType)
                {
                    FileDownloadName = fileName
                };
            }

            return BadRequest(new HttpError("Wrong request"));
        }

        /// <summary>
        /// Get data for export
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task<List<T>> GetDataAsync<T>(ExportInput input)
        {
            switch ((ExportDataType)input.ExportDataTypeId)
            {
                case ExportDataType.User:
                    return await _repositoryManager.User.GetUsers(input.DataIds, false) as List<T>;

                case ExportDataType.Unit:
                    return await _repositoryManager.Unit.GetUnits(input.DataIds) as List<T>;

                case ExportDataType.UnitReadings:
                    return await _repositoryManager.UnitReading.GetUnitReadings(input.DataIds, input.From, input.To, input.UnitId, false) as List<T>;
                default:
                    throw new InvalidOperationException("Unsupported export data type.");
            }
        }

        #endregion
    }
}
