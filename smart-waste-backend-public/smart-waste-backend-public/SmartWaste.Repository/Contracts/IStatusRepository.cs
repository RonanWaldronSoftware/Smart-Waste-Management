using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IStatusRepository
    {
        /// <summary>
        /// Get all statuses
        /// </summary>
        /// <returns></returns>
        public Task<List<Status>> GetStatuses();
    }
}
