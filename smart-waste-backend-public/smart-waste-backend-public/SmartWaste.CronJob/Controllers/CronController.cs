using SmartWaste.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartWaste.Model.Configuration;
using SmartWaste.Repository;
using SmartWaste.OldRepository;
using SmartWaste.Model.Core;
using SmartWaste.Model.OldCore;
using SmartWaste.Model.Constants;
using AutoMapper;
using SmartWaste.Model.Extensions;
using SmartWaste.Model.Enums;
using SmartWaste.CronJob.Extensions;
using Unit = SmartWaste.Model.Core.Unit;

namespace SmartWaste.CronJob.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CronController : ControllerBase
    {
        private readonly ILogger<CronController> _logger;
        private readonly IServiceManager _serviceManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IOldRepositoryManager _oldRepositoryManager;
        private readonly IMapper _mapper;
        private readonly Configuration _configuration;

        // Singleton class for holding a list of reported units
        private SingletonNotReporting _singletonNotReporting;

        private Dictionary<int, int?> oldNewUserIdsDict = new Dictionary<int, int?>();
        private Dictionary<int, int?> oldNewUnitIdsDict = new Dictionary<int, int?>();

        public CronController(ILogger<CronController> logger, IServiceManager serviceManager, IOptions<Configuration> configuration, IHttpContextAccessor accessor,
            IRepositoryManager repositoryManager, IOldRepositoryManager oldRepositoryManager, SingletonNotReporting singletonNotReporting, IMapper mapper)
        {
            _logger = logger;
            _serviceManager = serviceManager;
            _configuration = configuration.Value;
            _repositoryManager = repositoryManager;
            _oldRepositoryManager = oldRepositoryManager;
            _singletonNotReporting = singletonNotReporting;
            _mapper = mapper;

            // Add ip address and browser in scope
            _logger.BeginScope(new Dictionary<string, object>
            {
                ["ip_address"] = accessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                ["browser"] = accessor.HttpContext.Request.Headers.UserAgent.ToString()
            });
        }

        /// <summary>
        /// Delete one time tokens from database
        /// </summary>
        /// <returns></returns>
        [HttpGet("OneTimeTokens")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ClearOldOneTimeTokens()
        {
            try
            {
                // Delete old one time tokens
                await _serviceManager.AccountService.DeleteOldOneTimeTokens();

                _logger.LogInformation($"Old one time tokens deleted from database");

                return Ok();
            }
            catch (Exception ex)
            {
                using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(ex.ToString()))))
                {
                    _logger.LogError($"Error occurred while deleting old one time tokens");
                }
                return BadRequest();
            }
        }

        /// <summary>
        /// Send alerts for units not reporting
        /// </summary>
        /// <returns></returns>
        [HttpGet("NotReportingAlerts")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ReportUnitNotReporting()
        {
            // Get all units that are not reporting
            List<Model.Core.Unit> notReportingUnits = await _repositoryManager.Unit.GetNotReportingUnits();

            // Get not reporting alert
            AlertType notReportingAlertType = await _repositoryManager.AlertType.GetAlertType((int)AlertTypeEnum.DeviceNotReporting);

            // List of reported units in last call
            List<(int unitId, DateTime sendDate)> reportedUnits = _singletonNotReporting.GetReportedUnits();

            // List of units to stay in list
            List<int> unitsToStayInList = new List<int>();

            foreach (var unit in notReportingUnits)
            {
                try
                {
                    // Add unit to list
                    unitsToStayInList.Add(unit.Id);
                    if (!reportedUnits.Any(r => r.unitId.Equals(unit.Id)) || reportedUnits.Any(r => r.unitId.Equals(unit.Id) && r.sendDate.AddHours(24) <= DateTime.UtcNow))
                    {
                        // Remove unit from reported units if it's in the list
                        reportedUnits.Remove(reportedUnits.FirstOrDefault(r => r.unitId.Equals(unit.Id)));

                        // Add unit to reported units with new time
                        reportedUnits.Add((unit.Id, DateTime.UtcNow));

                        // Send messages
                        await _serviceManager.AlertService.SendAlert(unit.UserId.Value, new List<AlertType> { notReportingAlertType }, unit);
                    }
                }
                catch (Exception ex)
                {
                    using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(ex.ToString()))))
                    {
                        _logger.LogError($"Error occurred while sending alerts for unit {unit.Id}");
                    }
                }
            }

            // Remove all units that are not in a toStayList
            reportedUnits.RemoveAll(r => !unitsToStayInList.Any(ut => ut.Equals(r.unitId)));

            // Update singleton reported units
            _singletonNotReporting.ReplaceReportedUnits(reportedUnits);

            using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(reportedUnits.ToString()))))
            {
                _logger.LogError($"Not reporting alerts, logged units that stopped reporting");
            }

            return Ok();
        }

        /// <summary>
        /// Retrieves units with expired lock status from the repository, 
        /// Sets their BinLock to false and LockUntil to null, and saves the changes.
        /// </summary>
        /// <returns></returns>
        [HttpGet("UnlockUnit")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UnlockUnit()
        {
            _logger.LogInformation("Unlock units started");

            var units = await _repositoryManager.Unit.GetUnitsForUnlock();

            foreach (var unit in units)
            {
                unit.BinLock = false;
                unit.LockUntil = null;
                unit.LockCommandPending = true;
            }

            await _repositoryManager.SaveAsync();

            return Ok();
        }

        /// <summary>
        /// Database copy method
        /// </summary>
        /// <returns></returns>
        [HttpGet("RepopulateDatabase")]
        public async Task<IActionResult> RepopulateDatabase()
        {
            /*try
            {*/
                _logger.LogInformation("Database translation started");
                
                // Get all units from database
                List<Unit> units = await _repositoryManager.Unit.GetUnits();
                _logger.LogInformation($"Database translation - units for translation {units.Count}");

                foreach (var unit in units)
                {
                    // Get latest unit reading
                    var lastUnitReading = await _repositoryManager.UnitReading.GetLastUnitReading(unit.Id);

                    // Save new latest reading into latest reading table 
                    if (lastUnitReading != null)
                        await _serviceManager.UnitReadingService.AddLatestUnitReading(_mapper.Map<LatestUnitReading>(lastUnitReading));
                }

                _logger.LogInformation($"Database translation ended");

                return Ok();
            /*}
            catch (Exception ex)
            {
                using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(ex.ToString()))))
                {
                    _logger.LogError($"Database translation failed");
                }
                return BadRequest();
            }*/
        }

        #region private

        /// <summary>
        /// Create new users from old ones
        /// </summary>
        /// <param name="oldUsers"></param>
        /// <returns></returns>
        private async Task TranslateUsers(List<Model.OldCore.User> oldUsers)
        {
            // Populate new users
            List<Model.Core.User> newUsers = new List<Model.Core.User>();
            foreach (var user in oldUsers)
            {
                newUsers.Add(new Model.Core.User
                {
                    Name = user.Name,
                    Email = user.Email,
                    Password = user.Password,
                    ParentId = user.ParentId is null ? null : 1,
                    RoleId = user.Role.Value + 1,
                    StatusId = StatusSeter(user.Status),
                    LanguageId = LanguageSeter(user.Locale),
                    Address1 = user.Addr1,
                    Address2 = user.Addr2,
                    City = user.City,
                    County = user.County,
                    PostCode = user.Postcode,
                    CountryId = user.Country.Value + 100,
                    PhoneNumber = user.Mobile,
                    BinLevelAlert = user.BinLevelAlert,
                    LastLogin = user.LastLoggedIn,
                    CreateDate = user.InsertDate.Value,
                    UpdateDate = user.ModifiedDate.Value,
                    SubscriptionRenewalDate = user.SubRenewalDate,
                    Deleted = false,
                    CreateUserId = 1,
                    UpdateUserId = 1,
                });
            }

            // Create new user list
            newUsers = await _repositoryManager.User.CreateAListOfUsers(newUsers);

            // Set create and update users
            foreach (var oldUser in oldUsers)
            {
                // Populate dictionary with old and new user Ids for easy access
                oldNewUserIdsDict.Add(oldUser.Id, newUsers.First(u => u.Email.Equals(oldUser.Email)).Id);

                // Set parent
                Model.OldCore.User? oldParentUser = oldUsers.FirstOrDefault(u => u.Id == oldUser.ParentId);
                if (oldParentUser != null)
                    newUsers.First(u => u.Email.Equals(oldUser.Email)).ParentId = newUsers.First(u => u.Email.Equals(oldParentUser.Email)).Id;

                if(oldUser.Email.Equals("contact@example.invalid"))
                    newUsers.First(u => u.Email.Equals(oldUser.Email)).ParentId = null;

                // Set create user
                Model.OldCore.User oldCreateUser = oldUsers.First(u => u.Id == oldUser.InsertBy);
                newUsers.First(u => u.Email.Equals(oldUser.Email)).CreateUserId = newUsers.First(u => u.Email.Equals(oldCreateUser.Email)).Id;

                // Set last update user
                Model.OldCore.User? oldUpdateUser = oldUsers.FirstOrDefault(u => u.Id == oldUser.ModifiedBy);
                if (oldUpdateUser != null)
                    newUsers.First(u => u.Email.Equals(oldUser.Email)).UpdateUserId = newUsers.First(u => u.Email.Equals(oldUpdateUser.Email)).Id;
                else
                    newUsers.First(u => u.Email.Equals(oldUser.Email)).UpdateUserId = null;
            }

            // Update new user list
            newUsers = await _repositoryManager.User.UpdateAListOfUsers(newUsers);
        }

        /// <summary>
        /// Create new units from old ones
        /// </summary>
        /// <param name="oldUnits"></param>
        /// <returns></returns>
        private async Task<List<Model.Core.Unit>> TranslateUnits(List<Model.OldCore.Unit> oldUnits)
        {
            // Populate new units
            List<Model.Core.Unit> newUnits = new List<Model.Core.Unit>();
            foreach (var oldUnit in oldUnits)
            {
                newUnits.Add(new Model.Core.Unit
                {
                    UserId = oldNewUserIdsDict.FirstOrDefault(d => d.Key.Equals(oldUnit.OwnerId)).Value,
                    SerialNumber = oldUnit.SerialNo,
                    UnitModelTypeId = oldUnit.BinType,
                    Location = oldUnit.Location,
                    Latitude = oldUnit.Latitude is null ? null : (decimal)(oldUnit.Latitude),
                    Longitude = oldUnit.Longitude is null ? null : (decimal)(oldUnit.Longitude),
                    UnitTypeId = oldUnit.DeviceType,
                    ContentTypeId = oldUnit.ContentType,
                    EmptyLevel = oldUnit.EmptyLevel,
                    FullLevel = oldUnit.FullLevel,
                    StatusId = UnitStatusSeter(oldUnit.Status),
                    FirmWare = oldUnit.Firmware,
                    TimeDifference = oldUnit.TimeDiff,
                    UnitJustOn = oldUnit.BinJustOn is null ? false : oldUnit.BinJustOn == 0 ? false : true,
                    RegularPeriodicReporting = oldUnit.RegularPeriodicReporting is null ? false : oldUnit.RegularPeriodicReporting == 0 ? false : true,
                    NbloTissue = oldUnit.NbIoTissue is null ? false : oldUnit.NbIoTissue == 0 ? false : true,
                    HardwareRevision = oldUnit.HardwareRevision >= 0 && oldUnit.HardwareRevision <= 8 ? oldUnit.HardwareRevision + 1 : null,
                    CreateUserId = oldUnit.InsertBy is null ? null : oldNewUserIdsDict.FirstOrDefault(d => d.Key.Equals(oldUnit.InsertBy)).Value,
                    CreateDate = oldUnit.InsertDate is null ? DateTime.UtcNow : oldUnit.InsertDate.Value,
                    UpdateUserId = oldUnit.ModifiedBy is null ? null : oldNewUserIdsDict.FirstOrDefault(d => d.Key.Equals(oldUnit.ModifiedBy)).Value,
                    UpdateDate = oldUnit.ModifiedDate,
                    SigFoxAltId = oldUnit.SigfoxAltId,
                    SubscriptionRenewalDate = oldUnit.SubscriptionRenewalDate,
                    InstallationDate = oldUnit.InstallationDate
                });

                if (newUnits.Last().UserId == 0)
                    newUnits.Last().UserId = null;
                if (newUnits.Last().CreateUserId == 0)
                    newUnits.Last().CreateUserId = null;
                if (newUnits.Last().UpdateUserId == 0)
                    newUnits.Last().UpdateUserId = null;
            }

            // Create new units
            newUnits = await _repositoryManager.Unit.CreateAUnitList(newUnits);

            foreach (var unit in oldUnits)
            {
                // Populate old new id dictionary for easy access
                oldNewUnitIdsDict.Add(unit.Id, newUnits.First(u => u.SerialNumber.Equals(unit.SerialNo)).Id);
            }

            return newUnits;
        }

        /// <summary>
        /// Create new user alerts from old ones
        /// </summary>
        /// <param name="oldAlerts"></param>
        /// <returns></returns>
        private async Task TranslateUserAlerts(List<AlertDefn> oldAlerts)
        {
            List<UserAlert> newUserAlerts = new List<UserAlert>();

            foreach (var oldAlert in oldAlerts)
            {
                if (oldAlert.BinFull == 1) newUserAlerts.Add(PopulateAlert(oldAlert.UserId.Value, oldAlert.CustomerId.Value, DatabaseConstants.AlertType.BinFull.Id, oldAlert.InsertDate.Value));
                if (oldAlert.BatteryUvlo == 1) newUserAlerts.Add(PopulateAlert(oldAlert.UserId.Value, oldAlert.CustomerId.Value, DatabaseConstants.AlertType.BatteryUVLO.Id, oldAlert.InsertDate.Value));
                if (oldAlert.BinEmptiedLastPeriod == 1) newUserAlerts.Add(PopulateAlert(oldAlert.UserId.Value, oldAlert.CustomerId.Value, DatabaseConstants.AlertType.EmptiedLastPeriod.Id, oldAlert.InsertDate.Value));
                if (oldAlert.BatteryOverTempLo == 1) newUserAlerts.Add(PopulateAlert(oldAlert.UserId.Value, oldAlert.CustomerId.Value, DatabaseConstants.AlertType.BatteryOverTemperature.Id, oldAlert.InsertDate.Value));
                if (oldAlert.BinLocked == 1) newUserAlerts.Add(PopulateAlert(oldAlert.UserId.Value, oldAlert.CustomerId.Value, DatabaseConstants.AlertType.BinLocked.Id, oldAlert.InsertDate.Value));
                if (oldAlert.BinTilted == 1) newUserAlerts.Add(PopulateAlert(oldAlert.UserId.Value, oldAlert.CustomerId.Value, DatabaseConstants.AlertType.BinTilted.Id, oldAlert.InsertDate.Value));
                if (oldAlert.ServiceDoorOpen == 1) newUserAlerts.Add(PopulateAlert(oldAlert.UserId.Value, oldAlert.CustomerId.Value, DatabaseConstants.AlertType.ServiceDoorStuckOpen.Id, oldAlert.InsertDate.Value));
                if (oldAlert.FlapStuckOpen == 1) newUserAlerts.Add(PopulateAlert(oldAlert.UserId.Value, oldAlert.CustomerId.Value, DatabaseConstants.AlertType.FlapStuckOpen.Id, oldAlert.InsertDate.Value));
                if (oldAlert.Damage == 1) newUserAlerts.Add(PopulateAlert(oldAlert.UserId.Value, oldAlert.CustomerId.Value, DatabaseConstants.AlertType.DamageReport.Id, oldAlert.InsertDate.Value));
                if (oldAlert.NotReporting == 1) newUserAlerts.Add(PopulateAlert(oldAlert.UserId.Value, oldAlert.CustomerId.Value, DatabaseConstants.AlertType.DeviceNotReporting.Id, oldAlert.InsertDate.Value));
                if (oldAlert.Email == 1) newUserAlerts.Add(PopulateAlert(oldAlert.UserId.Value, oldAlert.CustomerId.Value, DatabaseConstants.AlertType.Email.Id, oldAlert.InsertDate.Value));
                if (oldAlert.Sms == 1) newUserAlerts.Add(PopulateAlert(oldAlert.UserId.Value, oldAlert.CustomerId.Value, DatabaseConstants.AlertType.SMS.Id, oldAlert.InsertDate.Value));
                if (oldAlert.Push == 1) newUserAlerts.Add(PopulateAlert(oldAlert.UserId.Value, oldAlert.CustomerId.Value, DatabaseConstants.AlertType.Push.Id, oldAlert.InsertDate.Value));
            }

            // Create a list of new user alerts
            await _repositoryManager.UserAlert.CreateUserAlerts(newUserAlerts);
        }

        /// <summary>
        /// Helper method for populating user alert
        /// </summary>
        /// <param name="oldUserId"></param>
        /// <param name="oldCustomerId"></param>
        /// <param name="alertType"></param>
        /// <param name="createDate"></param>
        /// <returns></returns>
        private UserAlert PopulateAlert(int oldUserId, int oldCustomerId, int alertType, DateTime createDate)
        {
            UserAlert userAlert = new UserAlert
            {
                AlertTypeId = alertType,
                UserId = oldNewUserIdsDict.First(d => d.Key.Equals(oldUserId)).Value,
                CustomerUserId = oldNewUserIdsDict.First(d => d.Key.Equals(oldCustomerId)).Value,
                CreateDate = createDate,
            };
            return userAlert;
        }

        /// <summary>
        /// Create a list of unit readings
        /// </summary>
        /// <param name="oldUnitReadings"></param>
        /// <returns></returns>
        private async Task TranslateUnitReadings(List<Model.OldCore.UnitReading> oldUnitReadings, Model.Core.Unit unit)
        {
            List<Model.Core.UnitReading> newUnitReadings = new List<Model.Core.UnitReading>();

            foreach (var oldUnitReading in oldUnitReadings)
            {
                var unitHardwareRevision = unit.HardwareRevision ?? null;

                newUnitReadings.Add(new Model.Core.UnitReading
                {
                    UnitId = unit.Id,
                    SerialNumber = oldUnitReading.SerialNo,
                    Source = oldUnitReading.Source,
                    MessageType = oldUnitReading.MsgType,
                    BinLevelBc = oldUnitReading.BinLevelBc,
                    BinLevel = oldUnitReading.BinLevel,
                    NoFlapOpenings = oldUnitReading.NoFlapOpenings is null ? null : (int)oldUnitReading.NoFlapOpenings,
                    BatteryVoltage = oldUnitReading.BatteryVoltage,
                    Temperature = oldUnitReading.Temperature,
                    NoCompactions = oldUnitReading.NoCompactions,
                    BatteryUvlo = oldUnitReading.BatteryUvlo is null ? false : oldUnitReading.BatteryUvlo <= 0 ? false : true,
                    BinEmptiedLastPeriod = oldUnitReading.BinEmptiedLastPeriod is null ? false : oldUnitReading.BinEmptiedLastPeriod <= 0 ? false : true,
                    BatteryOverTempLo = oldUnitReading.BatteryOverTempLo is null ? false : oldUnitReading.BatteryOverTempLo <= 0 ? false : true,
                    BinLocked = oldUnitReading.BinLocked is null ? false : oldUnitReading.BinLocked <= 0 ? false : true,
                    BinFull = oldUnitReading.BinFull is null ? false : oldUnitReading.BinFull <= 0 ? false : true,
                    BinTilted = oldUnitReading.BinTilted is null ? false : oldUnitReading.BinTilted <= 0 ? false : true,
                    ServiceDoorOpen = oldUnitReading.ServiceDoorOpen is null ? false : oldUnitReading.ServiceDoorOpen <= 0 ? false : true,
                    FlapStuckOpen = oldUnitReading.FlapStuckOpen is null ? false : oldUnitReading.FlapStuckOpen <= 0 ? false : true,
                    ServiceDoorClosed = oldUnitReading.ServiceDoorClosed is null ? false : oldUnitReading.ServiceDoorClosed <= 0 ? false : true,
                    NblotSignalStrength = oldUnitReading.NbIotSignalStrength,
                    Rssi = oldUnitReading.Rssi is null ? null : (decimal)oldUnitReading.Rssi,
                    Src = oldUnitReading.Src,
                    ContactReason = oldUnitReading.ContactReason,
                    AlarmStatus = oldUnitReading.AlarmStatus,
                    Snr = oldUnitReading.Snr is null ? null : oldUnitReading.Snr,
                    Ber = oldUnitReading.Ber,
                    Rsrq = oldUnitReading.Rsrq,
                    Rsrp = oldUnitReading.Rsrp,
                    Firmware = oldUnitReading.Firmware,
                    TimeDiff = oldUnitReading.TimeDiff,
                    UnitJustOn = oldUnitReading.BinJustOn is null ? false : oldUnitReading.BinJustOn <= 0 ? false : true,
                    RegularPeriodicReporting = oldUnitReading.RegularPeriodicReporting is null ? false : oldUnitReading.RegularPeriodicReporting <= 0 ? false : true,
                    NbloTissue = oldUnitReading.NbIoTissue is null ? false : oldUnitReading.NbIoTissue <= 0 ? false : true,
                    ReadingDate = oldUnitReading.ReadingDateTime,
                    CreateDate = oldUnitReading.InsertDateTime != null ? oldUnitReading.InsertDateTime.Value : DateTime.UtcNow,
                    HardwareRevision = unitHardwareRevision != null ? unitHardwareRevision >= 0 && unitHardwareRevision <= 7 ? unitHardwareRevision + 1 : null : null,
                });

                if (newUnitReadings.Last().UnitId != null)
                    newUnitReadings.Last().FullnessStatusId = unit.ComputeUnitFillLevelPercentages(newUnitReadings.Last()).FillnessStatusId;
                else
                    newUnitReadings.Last().FullnessStatusId = 12;
            }

            await _repositoryManager.UnitReading.CreateReadings(newUnitReadings);
        }

        /// <summary>
        /// Set status
        /// </summary>
        /// <param name="oldStatusId"></param>
        /// <returns></returns>
        private int StatusSeter(int? oldStatusId)
        {
            if (oldStatusId == 1)
                return DatabaseConstants.Status.Active.Id;
            else if (oldStatusId == 0)
                return DatabaseConstants.Status.Registered.Id;
            else
                return DatabaseConstants.Status.Inactive.Id;
        }

        /// <summary>
        /// Set unit status
        /// </summary>
        /// <param name="oldStatusId"></param>
        /// <returns></returns>
        private int UnitStatusSeter(int? oldStatusId)
        {
            if (oldStatusId == 1)
                return DatabaseConstants.UnitStatus.Active.Id;
            else if (oldStatusId == 0)
                return DatabaseConstants.UnitStatus.Registered.Id;
            else
                return DatabaseConstants.UnitStatus.Inactive.Id;
        }

        /// <summary>
        /// Set language
        /// </summary>
        /// <param name="oldlocale"></param>
        /// <returns></returns>
        private int LanguageSeter(string oldLocale)
        {
            if (oldLocale.Contains("hr"))
                return DatabaseConstants.Language.Croatian.Id;
            else if (oldLocale.Contains("en"))
                return DatabaseConstants.Language.English.Id;
            else if (oldLocale.Contains("fr"))
                return DatabaseConstants.Language.French.Id;
            else
                return DatabaseConstants.Language.German.Id;
        }
    }

    #endregion
}
