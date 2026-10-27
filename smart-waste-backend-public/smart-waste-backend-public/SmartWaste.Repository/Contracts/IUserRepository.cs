using SmartWaste.Model.Core;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Helpers;

namespace SmartWaste.Repository.Contracts
{
    public interface IUserRepository
    {
        /// <summary>
        /// Get user with user alerts
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public Task<User> GetUserWithAlerts(int userId, bool trackChanges = true);

        /// <summary>
        /// Check if user with provided email exists
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<bool> IsUserEmailExist(string email, int userId = 0);

        /// <summary>
        /// Check if user exists
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<bool> IsUserExist(int userId);

        /// <summary>
        /// Check if user has a one time token
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<User?> CheckForgotPasswordCode(string email, string token);

        /// <summary>
        /// Check if parent user can change child user password.
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="childId"></param>
        /// <returns></returns>
        public Task<bool> CanUserChangeData(int parentId, int childId);

        /// <summary>
        /// Get a list of users with alerts
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public Task<List<User>> GetUsersWithAlerts(List<int> userIds, int userId);

        /// <summary>
        /// Get a list of all user child ids. If roleids is not not filter only by tehnican and driver
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loggedInUser"></param>
        /// <param name="roleIds"></param>
        /// <param name="resetCheckList"></param>
        /// <returns></returns>
        public Task<List<int>> GetAllBranchUserChildrenIds(int userId, int loggedInUser, List<int>? roleIds = null, bool resetCheckList = true);

        /// <summary>
        /// Get a list of all user child ids. If roleids is not not filter only by tehnican and driver
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loggedInUser"></param>
        /// <param name="roleIds"></param>
        /// <param name="resetCheckList"></param>
        /// <returns></returns>
        public Task<List<User>> GetAllBranchUserChildren(int userId, int loggedInUser, List<int>? roleIds = null, bool resetCheckList = true);

        /// <summary>
        /// Get user Id
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<int> GetUserId(string email);

        /// <summary>
        /// Get user role id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<int?> GetUserRoleId(int userId);

        /// <summary>
        /// Get user by email and password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<User?> GetUser(string email, string password);

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<User?> GetUser(string email, bool trackChanges = true);

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<User?> GetUser(int userId, bool trackChanges = true);

        /// <summary>
        /// Get user for complete update with no extra includes
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<User?> GetUserForFullUpdate(int userId);

        /// <summary>
        /// Get users paginated
        /// </summary>
        /// <param name="options"></param>
        /// <param name="includeDeleted"></param>
        /// <param name="isAdmin"></param>
        /// <param name="loggedInUserId"></param>
        /// <param name="shortVersion"></param>
        /// <returns></returns>
        public Task<TableResponse<User>> GetUsersPaginated(DatatableOptions options, bool includeInactive, bool isAdmin, int loggedInUserId, bool shortVersion);

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task UpdateUser(User user);

        /// <summary>
        /// Update a list of users, used for database translation
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public Task<List<User>> UpdateAListOfUsers(List<User> users);

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task CreateUser(User user);

        /// <summary>
        /// Create a list of user, used for database translation
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public Task<List<User>> CreateAListOfUsers(List<User> users);

        /// <summary>
        /// Delete all user children
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task DeleteAllChildren(int userId);

        /// <summary>
        /// Add device token into database for sending push notification
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public Task AddDeviceToken(int userId, string token, int deviceType);

        /// <summary>
        /// Remove device token
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task RemoveUserTokens(string email);

        /// <summary>
        /// Get all possible parents for specific user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Task<List<User>> GetPossibleParents(int userId, PossibleParentTypeEnum type, int? roleId = null);
        /// <summary>
        /// Get possible parents ids only
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Task<List<int>> GetPossibleParentsIds(int userId, PossibleParentTypeEnum type, int? roleId = null);

        /// <summary>
        /// Get a list of admin Ids
        /// </summary>
        /// <returns></returns>
        public Task<List<int>> GetAllAdminIds();
        /// <summary>
        /// Get users with userids
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        Task<List<User>> GetUsers(List<int> userIds, bool trackChanges = true);
        /// <summary>
        /// Get users
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="loggedUserId"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public Task<List<int>> GetPossibleUserIds(bool isAdmin, int loggedUserId, bool trackChanges = true);

    }
}
