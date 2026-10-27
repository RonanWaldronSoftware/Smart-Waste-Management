using AutoMapper;
using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Model.DataTransferObjects;
using SmartWaste.Model.DataTransferObjects.Account;
using SmartWaste.Model.DataTransferObjects.Unit;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Error;
using SmartWaste.Model.Extensions;
using SmartWaste.Model.Helpers;
using SmartWaste.Model.JsonModels.Account;
using SmartWaste.Model.JsonModels.Unit;
using SmartWaste.Model.Resources;
using SmartWaste.Repository;
using SmartWaste.Service.Contracts;

namespace SmartWaste.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly LocalizerService _localizer;

        public UserService(IRepositoryManager repositoryManager, IMapper mapper, LocalizerService localizer)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<UserDto?> GetUser(int userId, int languageId)
        {
            // Get user
            User? user = await _repositoryManager.User.GetUser(userId);

            if (user is null)
                return null;

            // If user is not null map it
            UserDto mappedUser = _mapper.Map<UserDto>(user);
            Language language = await _repositoryManager.Language.GetLanguage(languageId);
            mappedUser.Status.Name = _localizer.GetLocalString(user.Status.TranslationKey, language.Code);
            mappedUser.Role.Name = _localizer.GetLocalString(user.Role.TranslationKey, language.Code);
            mappedUser.Country.Name = _localizer.GetLocalString(user.Country.TranslationKey, language.Code);
            mappedUser.Language.Name = _localizer.GetLocalString(user.Language.TranslationKey, language.Code);

            return mappedUser;
        }

        public async Task<PaginatedResponse<UserDto>> GetUsers(GetUsersInput input, int userRole, int languageId, int loggedInUserId)
        {
            // Map options
            DatatableOptions options = _mapper.Map<DatatableOptions>(input);

            // Get users as paginated table response
            TableResponse<User> tableUsers = await _repositoryManager.User.GetUsersPaginated(options, input.IncludeInactive, userRole == DatabaseConstants.Role.Admin.Id, loggedInUserId, input.SimpleVersion);

            // Map users to dtos
            List<UserDto> users = _mapper.Map<List<UserDto>>(tableUsers.Responses);

            // Translate translatable
            if (!input.SimpleVersion)
            {
                Language language = await _repositoryManager.Language.GetLanguage(languageId);
                foreach (var user in users)
                {
                    user.Country.Name = _localizer.GetLocalString(tableUsers.Responses.Where(u => u.Id == user.Id).Select(u => u.Country.TranslationKey).FirstOrDefault() ?? "", language.Code);
                    user.Language.Name = _localizer.GetLocalString(tableUsers.Responses.Where(u => u.Id == user.Id).Select(u => u.Language.TranslationKey).FirstOrDefault() ?? "", language.Code);
                    user.Status.Name = _localizer.GetLocalString(tableUsers.Responses.Where(u => u.Id == user.Id).Select(u => u.Status.TranslationKey).FirstOrDefault() ?? "", language.Code);
                    user.Role.Name = _localizer.GetLocalString(tableUsers.Responses.Where(u => u.Id == user.Id).Select(u => u.Role.TranslationKey).FirstOrDefault() ?? "", language.Code);
                }
            }

            return new PaginatedResponse<UserDto>() { Count = tableUsers.Options.TotalRecords, Items = users };
        }

        public async Task<ServiceExecutionResponse> DeleteUser(int loggedInUserId, string email)
        {
            // Get user for delete
            User? userForDelete = await _repositoryManager.User.GetUser(email);

            // Get logged in user with no tracking
            User? loggedInUser = await _repositoryManager.User.GetUser(loggedInUserId, false);

            // Check if bouth users are found and is the logged in user admin or parent of user for delete
            if (userForDelete is null || loggedInUser is null || (loggedInUser.RoleId != DatabaseConstants.Role.Admin.Id
                && !await _repositoryManager.User.CanUserChangeData(loggedInUser.Id, userForDelete.Id)))
                return new ServiceExecutionResponse() { IsError = true, ErrorMessage = _localizer.GetLocalString("cannotDeleteUserErrorMessage", loggedInUser.Language.Code)};

            userForDelete.Deleted = true;
            // Delete user
            await _repositoryManager.User.UpdateUser(userForDelete);
            // Delete all user children
            await _repositoryManager.User.DeleteAllChildren(userForDelete.Id);

            return new ServiceExecutionResponse() { IsError = false };

        }

        public async Task<ServiceExecutionResponse> CreateUser(CreateUserInput newUser, string createUserEmail, int languageId)
        {
            // Get language
            Language language = await _repositoryManager.Language.GetLanguage(languageId);

            // Get logged in user
            User? creatorUser = await _repositoryManager.User.GetUser(createUserEmail);

            // Get new user parent
            User? newUserParent = await _repositoryManager.User.GetUser(newUser.ParentId, false);

            if (await _repositoryManager.User.IsUserEmailExist(newUser.Email))
                return new ServiceExecutionResponse() { IsError = true, ErrorMessage = _localizer.GetLocalString("userWithEmailExistsErrorMessage", language.Code)};

            if (creatorUser is null || newUserParent is null || !CanCreateEditUser(creatorUser.RoleId, newUser.RoleId)
                || !(await _repositoryManager.User.GetPossibleParentsIds(creatorUser.Id, PossibleParentTypeEnum.User, newUser.RoleId)).Any(u => u == newUser.ParentId))
                return new ServiceExecutionResponse() { IsError = true, ErrorMessage = _localizer.GetLocalString("userNotAuthorizedToCreateErrorMessage", language.Code) };

            // Create new db user
            var newDbUser = _mapper.Map<User>(newUser);

            // Set user status to active
            newDbUser.StatusId = DatabaseConstants.Status.Registered.Id;

            // Set create user
            newDbUser.CreateUserId = creatorUser.Id;
            newDbUser.Password = newDbUser.Password.ToSHA512Hash();

            // Add new user into database
            await _repositoryManager.User.CreateUser(newDbUser);
            return new ServiceExecutionResponse() { IsError = false };
        }

        public async Task<ServiceExecutionResponse> UpdateUser(UpdateUserInput userUpdate, int loggedInUserId, int loggedInUserRole, int languageId)
        {
            // Get language
            Language language = await _repositoryManager.Language.GetLanguage(languageId);

            // If user is updating itself do not check parent and role; in this case old values are kept
            if (loggedInUserId != userUpdate.Id)
            {
                // Check if parent exists
                if (userUpdate.ParentId is not null && !await _repositoryManager.User.IsUserExist(userUpdate.ParentId.Value))
                    return new ServiceExecutionResponse() { IsError = true, ErrorMessage = _localizer.GetLocalString("parentUserNotFoundErrorMessage", language.Code) };

                // Check if user can have provided role
                if (!UserRoleExists(userUpdate.RoleId) && (await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId, PossibleParentTypeEnum.User, userUpdate.RoleId)).All(u => u != userUpdate.ParentId))
                    return new ServiceExecutionResponse() { IsError = true, ErrorMessage = _localizer.GetLocalString("roleDoesNotExistErrorMessage", language.Code) };
            }

            // Check logged in user can get user data
            if (loggedInUserRole != DatabaseConstants.Role.Admin.Id && !await _repositoryManager.User.CanUserChangeData(loggedInUserId, userUpdate.Id))
                return new ServiceExecutionResponse() { IsError = true, ErrorMessage = String.Format(_localizer.GetLocalString("userNotAuthorizedToUpdateErrorMessage", language.Code), userUpdate.Id) };

            // Check if email is used on another user
            if (await _repositoryManager.User.IsUserEmailExist(userUpdate.Email, userUpdate.Id))
                return new ServiceExecutionResponse() { IsError = true, ErrorMessage = _localizer.GetLocalString("userWithEmailExistsErrorMessage", language.Code) };

            // Get user for update
            User? user = await _repositoryManager.User.GetUserForFullUpdate(userUpdate.Id);

            // Check if user exists
            if (user == null)
                return new ServiceExecutionResponse() { IsError = true, ErrorMessage = _localizer.GetLocalString("userNotFoundErrorMessage", language.Code) };

            // If user is updating itself keep old role and parent
            if (loggedInUserId == userUpdate.Id)
            {
                userUpdate.RoleId = user.RoleId;
                userUpdate.ParentId = user.ParentId;
            }

            // Map user and set other values
            user = _mapper.Map(userUpdate, user);
            user.UpdateUserId = loggedInUserId;

            // Update user
            await _repositoryManager.User.UpdateUser(user);
            return new ServiceExecutionResponse() { IsError = false, };
        }

        public async Task<User?> UpdateUserLanguage(int userId, int languageId)
        {
            User? user = await _repositoryManager.User.GetUser(userId);

            if (user is not null)
            {
                // Update user language
                user.LanguageId = languageId;
                await _repositoryManager.User.UpdateUser(user);
            }

            return user;
        }

        public async Task<ServiceExecutionResponse> ChangeUserStatus(int loggedInUserRole, int loggedInUserId, int updateUserId, int languageId)
        {
            // Get language
            Language language = await _repositoryManager.Language.GetLanguage(languageId);

            // Check logged in user can get user data
            if (loggedInUserId != updateUserId && loggedInUserRole != DatabaseConstants.Role.Admin.Id && !await _repositoryManager.User.CanUserChangeData(loggedInUserId, updateUserId))
                return new ServiceExecutionResponse()
                {
                    IsError = true,
                    ErrorMessage = String.Format(_localizer.GetLocalString("userNotAuthorizedToChangeStatusErrorMessage", language.Code), updateUserId)
                };

            // Get user
            User? user = await _repositoryManager.User.GetUser(updateUserId);

            if (user is null)
                return new ServiceExecutionResponse()
                {
                    IsError = true,
                    ErrorMessage = String.Format(_localizer.GetLocalString("userNotAuthorizedToChangeStatusErrorMessage", language.Code), updateUserId)
                };

            // Check and set user status
            if (user.StatusId == DatabaseConstants.Status.Inactive.Id)
                user.StatusId = DatabaseConstants.Status.Registered.Id;
            else
                user.StatusId = DatabaseConstants.Status.Inactive.Id;

            // Update user
            await _repositoryManager.User.UpdateUser(user);

            return new ServiceExecutionResponse()
            {
                IsError = false,
            };
        }

        public async Task<ServiceExecutionResponse> CreateUserAlert(List<UserAlertInput> inputs, int loggedInUserId, int languageId)
        {
            // Initialize a list for old alert delete
            List<(int, int)> oldAlertList = new List<(int, int)>();

            // Initialize a new list of UserAlerts
            List<UserAlert> userAlerts = new List<UserAlert>();

            foreach (var input in inputs)
            {
                // Get user roles
                int? loggedInUserRoleId = await _repositoryManager.User.GetUserRoleId(loggedInUserId);
                int? alertUserRoleId = await _repositoryManager.User.GetUserRoleId(input.UserId);
                int? binUserRoleId = await _repositoryManager.User.GetUserRoleId(input.CustomerUserId);

                // Check if user alert can be created
                if (loggedInUserRoleId is null || alertUserRoleId is null || binUserRoleId is null
                    || ((loggedInUserRoleId == DatabaseConstants.Role.Technician.Id || loggedInUserRoleId == DatabaseConstants.Role.Technician.Id) &&
                    loggedInUserId != input.UserId) || (loggedInUserId != input.UserId
                    && ((await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId, PossibleParentTypeEnum.Unit, alertUserRoleId)).All(u => u != input.CustomerUserId)
                        || (await GetBranchChildIds(input.CustomerUserId, loggedInUserId, loggedInUserRoleId.Value)).All(id => id != input.UserId))))
                {
                    Language language = await _repositoryManager.Language.GetLanguage(languageId);
                    return new ServiceExecutionResponse()
                    {
                        IsError = true,
                        ErrorMessage = _localizer.GetLocalString("userCanNotCreateAlertErrorMessage", language.Code)
                    };
                }

                // Add tuple to old alert list
                oldAlertList.Add((input.UserId, input.CustomerUserId));

                // Populate user alert list
                userAlerts.AddRange(input.AlertTypeIds.Select(alertType => new UserAlert { AlertTypeId = alertType, UserId = input.UserId, CustomerUserId = input.CustomerUserId, }).ToList());

            }

            List<UserAlert>? oldAlerts = await _repositoryManager.UserAlert.GetUserAlerts(oldAlertList, true);

            // Delete all old alerts
            await _repositoryManager.UserAlert.DeleteAlerts(oldAlerts);

            // Create new user alert
            await _repositoryManager.UserAlert.CreateUserAlerts(userAlerts);

            return new ServiceExecutionResponse()
            {
                IsError = false
            };
        }

        public async Task<List<AlertUserDto>?> GetAlertsWithUsers(int loggedInUserId, int customerUserId, int languageId)
        {
            // Get relevant users
            int? loggedInUserRoleId = await _repositoryManager.User.GetUserRoleId(loggedInUserId);
            int? userRoleId = await _repositoryManager.User.GetUserRoleId(customerUserId);
            
            // Check if loggedIn user can view users data
            if (loggedInUserRoleId is null || userRoleId is null || (loggedInUserId != customerUserId 
                && (await _repositoryManager.User.GetPossibleParentsIds(loggedInUserId, PossibleParentTypeEnum.Alert)).All(u => u != customerUserId)))
                return null;

            // Get user childrenIds (if logged in user is driver or technician pass only that users id)
            List<int> childUserIds = loggedInUserRoleId == DatabaseConstants.Role.Driver.Id || loggedInUserRoleId == DatabaseConstants.Role.Technician.Id ?
                new List<int>() { loggedInUserId } : await GetBranchChildIds(customerUserId, loggedInUserId, loggedInUserRoleId.Value);

            // Get alerts with users
            List<User> userAlerts = await _repositoryManager.User.GetUsersWithAlerts(childUserIds, customerUserId);

            // Get language
            Language language = await _repositoryManager.Language.GetLanguage(languageId);

            foreach (var item in userAlerts.SelectMany(user => user.UserAlertUsers))
            {
                item.AlertType.Name = _localizer.GetLocalString(item.AlertType.TranslationKey ?? "", language.Code);
            }

            // Create response alerts with users
            List<AlertUserDto> alertsResponse = _mapper.Map<List<AlertUserDto>>(userAlerts);

            return alertsResponse;
        }

        #region private

        /// <summary>
        /// This method determines which roles have the authority to create other roles
        /// </summary>
        /// <param name="editorUserRoleId"></param>
        /// <param name="newUserRoleId"></param>
        /// <returns></returns>
        private bool CanCreateEditUser(int editorUserRoleId, int newUserRoleId)
        {
            // Set of invalid roles, determines if a user with certain role can edit another user
            var invalidNewRoles = new Dictionary<int, HashSet<int>>()
            {
                { RoleConstantConstants.DistributorRoleId, [DatabaseConstants.Role.Admin.Id, DatabaseConstants.Role.Distributor.Id] },
                { RoleConstantConstants.CorporateCustomerRoleId, [DatabaseConstants.Role.Admin.Id, DatabaseConstants.Role.Distributor.Id, DatabaseConstants.Role.CorporateCustomer.Id] },
                { RoleConstantConstants.CustomerRoleId, [DatabaseConstants.Role.Admin.Id, DatabaseConstants.Role.Distributor.Id, DatabaseConstants.Role.CorporateCustomer.Id, DatabaseConstants.Role.Customer.Id] }
            };

            return !invalidNewRoles.TryGetValue(editorUserRoleId, out var invalidRoles) || !invalidRoles.Contains(newUserRoleId) || editorUserRoleId == RoleConstantConstants.AdminRoleId;
        }

        /// <summary>
        /// Check if user role exists
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private bool UserRoleExists(int roleId)
        {
            HashSet<int> validRoleIds = new HashSet<int>
            {
                DatabaseConstants.Role.Admin.Id,
                DatabaseConstants.Role.Distributor.Id,
                DatabaseConstants.Role.CorporateCustomer.Id,
                DatabaseConstants.Role.Customer.Id,
                DatabaseConstants.Role.Driver.Id,
                DatabaseConstants.Role.Technician.Id
            };

            return validRoleIds.Contains(roleId);
        }

        /// <summary>
        /// Get children from a single branch
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loggedInUserId"></param>
        /// <param name="loggedInUserRoleId"></param>
        /// <returns></returns>
        private async Task<List<int>> GetBranchChildIds(int userId, int loggedInUserId, int loggedInUserRoleId)
        {
            // Get user childrenIds with drivers and technicians included
            List<int> childUserIds = await _repositoryManager.User.GetAllBranchUserChildrenIds(userId, loggedInUserId);

            // Add all admins
            if (loggedInUserRoleId == DatabaseConstants.Role.Admin.Id)
                childUserIds.AddRange(await _repositoryManager.User.GetAllAdminIds());

            // Remove duplicates
            List<int> distinctChildUserIds = childUserIds.Distinct().OrderBy(id => id).ToList();

            return distinctChildUserIds;
        }

        #endregion
    }
}
