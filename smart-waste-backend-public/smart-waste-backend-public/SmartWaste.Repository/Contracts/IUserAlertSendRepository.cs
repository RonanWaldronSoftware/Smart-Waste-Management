using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IUserAlertSendRepository
    {
        /// <summary>
        /// Create user send alert
        /// </summary>
        /// <param name="userAlertSend"></param>
        /// <returns></returns>
        public Task CreateUserSendAlert(UserAlertSend userAlertSend);
        /// <summary>
        /// The method takes a list of AlertType as input. It retrieves the IDs of the specified alert types and filters out user alert sends (UserAlertSend) that do not match these IDs.
        /// The method then deletes these filtered user alerts send from the database.
        /// </summary>
        /// <param name="excludingAlertTypes"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task DeleteUserAlertsSendExcludingTypes(List<AlertType> excludingAlertTypes, int userId, int unitId);
    }
}
