using SmartWaste.Model.Core;
using SmartWaste.Model.DataTransferObjects.Dashboard;
using SmartWaste.Model.Helpers;
using SmartWaste.Model.JsonModels.Unit;

namespace SmartWaste.Repository.Contracts
{
    public interface IUnitRepository
    {
        /// <summary>
        /// Create a list of units for database translation
        /// </summary>
        /// <param name="newUnits"></param>
        /// <returns></returns>
        public Task<List<Unit>> CreateAUnitList(List<Unit> newUnits);

        /// <summary>
        /// Create unit async
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public Task CreateUnitAsync(Unit unit);

        /// <summary>
        /// Create unit
        /// </summary>
        /// <param name="unit"></param>
        public void CreateUnit(Unit unit);

        /// <summary>
        /// Get unit by serial number
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="trackChanges"></param>
        /// <param name="isSigfox"></param>
        /// <returns></returns>
        public Task<Unit> GetUnit(string serialNumber, bool trackChanges = true, bool isSigfox = false);

        /// <summary>
        /// Get unit by Id
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="trackChanges"></param>
        /// <param name="includeRelated"></param>
        /// <returns></returns>
        public Task<Unit?> GetUnit(int unitId, bool trackChanges = true, bool includeRelated = true);

        /// <summary>
        /// Get 
        /// </summary>
        /// <returns></returns>
        public Task<List<Unit>> GetNotReportingUnits();

        /// <summary>
        /// Get units as table response
        /// </summary>
        /// <param name="input"></param>
        /// <param name="possibleParentsIds"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public Task<TableResponse<Unit>> GetUnits(GetUnitsInput input, List<int> possibleParentsIds, bool trackChanges = true);

        /// <summary>
        /// Get active units for customer
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public Task<List<Unit>> GetUnitsForCustomer(int userid, bool trackChanges = true);

        /// <summary>
        /// Check if unit with serial number exists
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public Task<bool> IsSerialNumberExist(string serialNumber);

        /// <summary>
        /// Get unit count for units which has any alert
        /// </summary>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public Task<int> GetUnitNavigationCount(List<int> possibleParentsIds, bool trackChanges = true);

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        /// <param name="possibleParentsIds"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public Task<DashboardDto> GetDashboardStatistic(List<int> possibleParentsIds, bool trackChanges = true);

        /// <summary>
        /// Get a list of units for export
        /// </summary>
        /// <param name="unitsIds"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public Task<List<Unit>> GetUnits(List<int> unitsIds, bool trackChanges = true);
        /// <summary>
        /// Lock bin
        /// </summary>
        /// <param name="input"></param>
        /// <param name="loggedInUserId"></param>
        /// <returns></returns>
        Task LockBin(Unit unit, BinLockInput input, int loggedInUserId);
        /// <summary>
        /// Get units where lock time expired
        /// </summary>
        /// <returns></returns>
        Task<List<Unit>> GetUnitsForUnlock(bool trackChanges = true);

        /// <summary>
        /// Get all units from database
        /// </summary>
        /// <returns></returns>
        Task<List<Unit>> GetUnits();

        /// <summary>
        /// Check if unit have readings
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        Task<bool> CheckIfUnitHaveReadings(int unitId);
        /// <summary>
        /// Check if any serial number already exist
        /// </summary>
        /// <param name="serialNumbers"></param>
        /// <returns></returns>
        Task<bool> CheckIfSerialNumbersExist(List<string> serialNumbers);
    }
}
