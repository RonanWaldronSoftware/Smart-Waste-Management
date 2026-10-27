using SmartWaste.Model.Core;
using SmartWaste.Model.DataTransferObjects;
using SmartWaste.Model.DataTransferObjects.Account;
using SmartWaste.Model.DataTransferObjects.Unit;
using SmartWaste.Model.Error;
using SmartWaste.Model.JsonModels.Account;
using SmartWaste.Model.JsonModels.Unit;

namespace SmartWaste.Service.Contracts
{
    public interface IUserService
    {
        /// <summary>
        /// Get user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        public Task<UserDto?> GetUser(int userId, int languageId);

        /// <summary>
        /// Get users paginated
        /// </summary>
        /// <param name="input"></param>
        /// <param name="userRole"></param>
        /// <param name="languageId"></param>
        /// <param name="loggedInUserId"></param>
        /// <returns></returns>
        public Task<PaginatedResponse<UserDto>> GetUsers(GetUsersInput input, int userRole, int languageId, int loggedInUserId);

        /// <summary>
        /// Delete user and it's sub users
        /// </summary>
        /// <param name="loggedInUserId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<ServiceExecutionResponse> DeleteUser(int loggedInUserId, string email);

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="newUser"></param>
        /// <param name="createUserEmail"></param>
        /// <returns></returns>
        public Task<ServiceExecutionResponse> CreateUser(CreateUserInput newUser, string createUserEmail, int languageId);

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="userUpdate"></param>
        /// <returns></returns>
        public Task<ServiceExecutionResponse> UpdateUser(UpdateUserInput userUpdate, int loggedInUserId, int loggedInUserRole, int languageId);

        /// <summary>
        /// Change user status to active or inactive
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="loggedInUserRole"></param>
        /// <param name="loggedInUserId"></param>
        /// <param name="updateUserId"></param>
        /// <returns></returns>
        public Task<ServiceExecutionResponse> ChangeUserStatus(int loggedInUserRole, int loggedInUserId, int updateUserId, int languageId);

        /// <summary>
        /// Update user language
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        public Task<User?> UpdateUserLanguage(int userId, int languageId);

        /// <summary>
        /// Create a new user alert
        /// </summary>
        /// <param name="userAlerts"></param>
        /// <param name="loggedInUserId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        public Task<ServiceExecutionResponse> CreateUserAlert(List<UserAlertInput> userAlerts, int loggedInUserId, int languageId);

        /// <summary>
        /// Get a list of alerts with users
        /// </summary>
        /// <param name="loggedInUserId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<List<AlertUserDto>?> GetAlertsWithUsers(int loggedInUserId, int userId, int languageId);
    }
}
