using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IUserAlertRepository
    {
        /// <summary>
        /// Get a list of user alerts
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public Task<List<UserAlert>?> GetUserAlerts(List<(int userId, int customerUserId)> userUserCustomerIds, bool trackChanges = true);

        /// <summary>
        /// Create a new user alert
        /// </summary>
        /// <param name="newUserAlert"></param>
        /// <returns></returns>
        public Task CreateUserAlerts(List<UserAlert> newUserAlert);

        /// <summary>
        /// Delete user alerst
        /// </summary>
        /// <param name="userAlerts"></param>
        /// <returns></returns>
        public Task DeleteAlerts(List<UserAlert> userAlerts);
    }
}
