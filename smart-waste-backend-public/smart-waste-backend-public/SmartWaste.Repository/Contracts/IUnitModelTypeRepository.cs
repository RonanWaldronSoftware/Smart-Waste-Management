using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IUnitModelTypeRepository
    {
        /// <summary>
        /// Get all unit model types
        /// </summary>
        /// <returns></returns>
        public Task<List<UnitModelType>> GetUnitModelTypes();
    }
}
