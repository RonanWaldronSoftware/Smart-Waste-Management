using SmartWaste.Model.Core;
using SmartWaste.Model.Helpers;
using SmartWaste.Model.JsonModels.Damage;

namespace SmartWaste.Repository.Contracts
{
    public interface IDamageRepository
    {
        /// <summary>
        /// Create damage and damage history
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="unitId"></param>
        /// <param name="uploadedImage"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task CreateDamage(CreateDamageInput damage, int unitId, string uploadedImage, int userId);
        /// <summary>
        /// Gets a filtered, paginated list of active damages with detailed includes, based on search criteria and specified options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="possiblePerentIds"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public Task<TableResponse<Damage>> GetActiveDamages(DatatableOptions options, List<int> possiblePerentIds, bool trackChanges = true);
        /// <summary>
        /// Get damage by id
        /// </summary>
        /// <param name="damageId"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        Task<Damage?> GetDamage(int damageId, bool trackChanges = true);
        /// <summary>
        /// Get damage navigation count only for damage status open and assigned
        /// </summary>
        /// <param name="possibleParentsIds"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        Task<int> GetNavigationCount(List<int> possibleParentsIds, bool trackChanges = true);
        /// <summary>
        /// Gets a list of damages for a specific unit
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        Task<List<Damage>> GetUnitDamages(int unitId, bool trackChanges = true);
    }
}
