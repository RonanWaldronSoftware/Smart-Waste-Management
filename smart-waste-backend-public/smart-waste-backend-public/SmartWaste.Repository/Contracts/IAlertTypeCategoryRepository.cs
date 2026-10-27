using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IAlertTypeCategoryRepository
    {
        /// <summary>
        /// Get all alert type categories
        /// </summary>
        /// <returns></returns>
        public Task<List<AlertTypeCategory>> GetAlertTypeCategories();
    }
}
