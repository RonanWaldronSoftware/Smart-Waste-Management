using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IDamageHistoryRepository
    {
        /// <summary>
        /// Get unit damage history
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public Task<List<DamageHistory>> GetUnitDamageHistory(int unitId, bool trackChanges = true);
    }
}
