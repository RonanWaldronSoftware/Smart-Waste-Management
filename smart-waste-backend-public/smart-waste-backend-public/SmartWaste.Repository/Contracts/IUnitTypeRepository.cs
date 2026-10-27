using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IUnitTypeRepository
    {
        /// <summary>
        /// Get all unit types
        /// </summary>
        /// <returns></returns>
        public Task<List<UnitType>> GetUnitTypes();
    }
}
