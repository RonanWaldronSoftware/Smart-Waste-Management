using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IStatusCategoryRepository
    {
        /// <summary>
        /// Get all status categories
        /// </summary>
        /// <returns></returns>
        public Task<List<StatusCategory>> GetStatusCategories();
    }
}
