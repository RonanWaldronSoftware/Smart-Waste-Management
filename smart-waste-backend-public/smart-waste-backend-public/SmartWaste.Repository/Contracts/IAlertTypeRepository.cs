using SmartWaste.Model.Core;
using SmartWaste.Model.Enums;

namespace SmartWaste.Repository.Contracts
{
    public interface IAlertTypeRepository
    {
        /// <summary>
        /// Get all alert types
        /// </summary>
        /// <returns></returns>
        public Task<List<AlertType>> GetAlertTypes();

        /// <summary>
        /// Get all alerts by category
        /// </summary>
        /// <param name="categoryEnum"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public Task<List<AlertType>> GetAlertTypes(AlertTypeCategoryEnum categoryEnum, bool trackChanges = true);

        /// <summary>
        /// Get alerts with users
        /// </summary>
        /// <returns></returns>
        public Task<List<AlertType>> GetAlertsWithUser(List<int> childUserIds);
        /// <summary>
        /// Get alert type by id
        /// </summary>
        /// <param name="alertTypeId"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        Task<AlertType> GetAlertType(int alertTypeId, bool trackChanges = true);
    }
}
