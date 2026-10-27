using SmartWaste.Model.Core;
using SmartWaste.Model.Helpers;
using SmartWaste.Model.JsonModels.Unit;

namespace SmartWaste.Repository.Contracts
{
    public interface IUnitReadingRepository
    {
        /// <summary>
        /// Create unit reading
        /// </summary>
        /// <param name="unitReading"></param>
        /// <returns></returns>
        Task<UnitReading> CreateReading(UnitReading unitReading);

        /// <summary>
        /// Create a list of unit readings used for database translation
        /// </summary>
        /// <param name="unitReadings"></param>
        /// <returns></returns>
        Task CreateReadings(List<UnitReading> unitReadings);

        /// <summary>
        /// Get unit readings where there is not unitId for this serialNumbers
        /// </summary>
        /// <param name="serialNumbers"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        Task<List<UnitReading>> GetUnassignedReadings(List<string> serialNumbers, bool trackChanges = true);

        /// <summary>
        /// Get unit reading
        /// </summary>
        /// <param name="id"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        Task<UnitReading?> GetUnitReading(int id, bool trackChanges = true);

        /// <summary>
        /// Retrieves unit readings based on the specified criteria.
        /// If <paramref name="options.forLastDays"/> is provided, returns readings from the last specified number of days.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        Task<TableResponse<UnitReading>> GetUnitReadings(UnitReadingDatatableOptions options, bool trackChanges = true);

        /// <summary>
        /// Gets a list of unit readings filtered by IDs or date range if provided.
        /// </summary>
        /// <param name="dataIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="unitId"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        Task<List<UnitReading>> GetUnitReadings(List<int> dataIds, DateTime? from, DateTime? to, int? unitId, bool trackChanges = true);

        /// <summary>
        /// Get latest unit reading from unit readings table
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        Task<UnitReading?> GetLastUnitReading(int unitId);

        /// <summary>
        /// Save latest unit reading into database for specific table
        /// </summary>
        /// <param name="latestUnitReading"></param>
        /// <returns></returns>
        Task AddNewLatestReading(LatestUnitReading latestUnitReading);

        /// <summary>
        /// Get latest unit reading from latest unit reading table
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        Task<LatestUnitReading?> GetLatestUnitReadingFromLatestUnitReading(int unitId, bool trackChanges = true);
    }
}
