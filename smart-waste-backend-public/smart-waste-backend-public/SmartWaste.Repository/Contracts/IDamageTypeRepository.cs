using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IDamageTypeRepository
    {
        /// <summary>
        /// Get all damage types
        /// </summary>
        /// <returns></returns>
        public Task<List<DamageType>> GetDamageTypes();
    }
}
