using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Extensions;
using SmartWaste.Model.Helpers;
using SmartWaste.Model.Static;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
using System.Diagnostics;

namespace SmartWaste.Repository.Implementation
{
    internal sealed class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly RepositoryContext _repositoryContext;
        private List<int> _checkedUsers = [];

        public UserRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        #region Check
        public async Task<bool> IsUserEmailExist(string email, int userId = 0) => await _repositoryContext.Users
                .AnyAsync(u => u.Email!.Equals(email) && u.Deleted == false && (userId == 0 || !u.Id.Equals(userId)));

        public async Task<bool> IsUserExist(int userId) => await _repositoryContext.Users.AnyAsync(u => u.Id.Equals(userId) && u.Deleted == false);

        public async Task<User?> CheckForgotPasswordCode(string email, string token)
            => await _repositoryContext.Users.FirstOrDefaultAsync(u => u.Email!.Equals(email) && u.OneTimeTokens.Any(t => t.Token.Equals(token)) && u.Deleted == false);

        public async Task<bool> CanUserChangeData(int userId, int childId)
        {
            // Get users that have parentId as parentId
            List<User> childUsers = await _repositoryContext.Users.Where(u => u.ParentId == userId).ToListAsync();
            foreach (var childUser in childUsers)
            {
                // Check if child user is same like childId
                if (childUser.Id == childId)
                    return true;
                else
                {
                    // Check if child children are same like child id
                    bool canChange = await CanUserChangeData(childUser.Id, childId);
                    if (canChange)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region User device tokens

        public async Task AddDeviceToken(int userId, string token, int deviceType)
        {
            // Check if device token exist
            if (!await _repositoryContext.DeviceTokens.AnyAsync(t => t.UserId == userId && token.Equals(token)))
            {
                // Create new token and add into database
                _repositoryContext.DeviceTokens.Add(new DeviceToken
                {
                    UserId = userId,
                    DeviceToken1 = token,
                    DeviceTypeId = deviceType
                });

                // Save changes
                await _repositoryContext.SaveChangesAsync();
            }
        }

        public async Task RemoveUserTokens(string email)
        {
            // Get all tokens for user
            var tokens = await _repositoryContext.DeviceTokens.Where(d => d.User.Email.Equals(email)).ToListAsync();

            // Remove all tokens
            _repositoryContext.DeviceTokens.RemoveRange(tokens);

            await _repositoryContext.SaveChangesAsync();
        }

        #endregion

        #region Get

        public async Task<User> GetUserWithAlerts(int userId, bool trackChanges = true)
        => await FindByCondition(u => u.Id == userId, trackChanges)
            .Include(u => u.Language)
            .Include(u => u.UserAlertCustomerUsers)
            .Include(u => u.UserAlertUsers).ThenInclude(ua => ua.UserAlertSends)
            .Include(u => u.InverseParent)
            .Include(u => u.DeviceTokens)
            .Include(u => u.Country)
            .FirstOrDefaultAsync();

        public async Task<List<User>> GetUsersWithAlerts(List<int> userIds, int userId) => await _repositoryContext.Users
            .Include(u => u.UserAlertUsers.Where(uau => uau.CustomerUserId == userId))
            .ThenInclude(ua => ua.AlertType).Where(u => userIds.Any(uId => uId == u.Id) && u.Deleted == false).ToListAsync();

        public async Task<List<int>> GetAllBranchUserChildrenIds(int userId, int loggedInUser, List<int> roleIds = null, bool resetCheckList = true)
        {
            // Ensure roleIds is not null
            roleIds ??= new List<int>();

            // Check if need to reset list
            if (resetCheckList)
                _checkedUsers = [];

            // Get all child users for selected user
            List<int> userIds = new List<int>();
            var user = await _repositoryContext.Users.Where(u => u.Id == userId && u.Deleted == false).Include(u => u.InverseParent).FirstOrDefaultAsync();

            if (user != null)
            {
                // Add the current user's ID to the list if roleIds is empty or contains the user's role ID
                if (!roleIds.Any() || roleIds.Contains(user.RoleId))
                    userIds.Add(userId);

                // Get children
                var children = roleIds.Any()
                     ? user.InverseParent.Where(c => roleIds.Contains(c.RoleId) && !c.Deleted)
                     : user.InverseParent.Where(c => (c.RoleId == DatabaseConstants.Role.Technician.Id || c.RoleId == DatabaseConstants.Role.Driver.Id) && !c.Deleted);

                // Add user driver and technicians to list
                userIds.AddRange(children.Select(child => child.Id));

                // Get all users child
                if (user.ParentId != null && userId != loggedInUser && _checkedUsers.All(c => c != user.ParentId))
                {
                    Debug.WriteLine("Loop -> " + user.ParentId);
                    _checkedUsers.Add(user.ParentId ?? 0);
                    userIds.AddRange(await GetAllBranchUserChildrenIds(user.ParentId.Value, loggedInUser, roleIds, false));
                }

            }

            return userIds;
        }

        public async Task<List<User>> GetAllBranchUserChildren(int userId, int loggedInUser, List<int> roleIds = null, bool resetCheckList = true)
        {
            // Ensure roleIds is not null
            roleIds ??= new List<int>();

            // Check if need to reset list
            if (resetCheckList)
                _checkedUsers = [];

            // Get all child users for selected user
            List<User> users = new List<User>();
            var user = await _repositoryContext.Users
                .Where(u => u.Id == userId && !u.Deleted)
                .Include(u => u.InverseParent)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                // Add the current user to the list if roleIds is empty or contains the user's role ID
                if (!roleIds.Any() || roleIds.Contains(user.RoleId))
                    users.Add(user);

                // Get children
                var children = roleIds.Any()
                    ? user.InverseParent.Where(c => roleIds.Contains(c.RoleId) && !c.Deleted)
                    : user.InverseParent.Where(c => (c.RoleId == DatabaseConstants.Role.Technician.Id || c.RoleId == DatabaseConstants.Role.Driver.Id) && !c.Deleted);

                // Add user driver and technicians to list
                users.AddRange(children);

                // Get all user's parents recursively if applicable
                if (user.ParentId != null && userId != loggedInUser && _checkedUsers.All(c => c != user.ParentId))
                {
                    _checkedUsers.Add(user.ParentId ?? 0);
                    users.AddRange(await GetAllBranchUserChildren(user.ParentId.Value, loggedInUser, roleIds));
                }
            }

            return users;
        }

        public async Task<List<User>> GetPossibleParents(int userId, PossibleParentTypeEnum type, int? roleId = null)
        {
            // Get user from database
            var user = await GetUser(userId, false);

            if (user == null) return [];


            if (type is PossibleParentTypeEnum.Alert or PossibleParentTypeEnum.Route or PossibleParentTypeEnum.Unit
                && (user.RoleId == DatabaseConstants.Role.Technician.Id
                || user.RoleId == DatabaseConstants.Role.Driver.Id))
            {
                user = await GetUser(user.ParentId.Value, false);
            }

            List<User> users = new();
            List<int> availableRoles = RoleStaticService.GetPossibleRolesByType(type, roleId);

            // Check if it's role admin or any other.
            if (DatabaseConstants.Role.Admin.Id == user.RoleId)
                users = await _repositoryContext.Users.Where(u => !u.Deleted && u.StatusId != DatabaseConstants.Status.Inactive.Id && availableRoles.Any(r => r == u.RoleId)).AsNoTracking().ToListAsync();
            else
                users = await GetAllUserChild(user.Id);

            users = users.Where(u => availableRoles.Any(r => r == u.RoleId)).OrderBy(u => u.Name).ToList();

            return users;
        }

        public async Task<List<int>> GetPossibleParentsIds(int userId, PossibleParentTypeEnum type, int? roleId = null)
        {
            // Get user from database
            var user = await GetUser(userId, false);

            if (user == null) return new List<int>();

            if (type is PossibleParentTypeEnum.Alert or PossibleParentTypeEnum.Route or PossibleParentTypeEnum.Unit
                && (user.RoleId == DatabaseConstants.Role.Technician.Id
                || user.RoleId == DatabaseConstants.Role.Driver.Id))
            {
                user = await GetUser(user.ParentId.Value, false);
            }

            List<User> users = new();
            List<int> availableRoles = RoleStaticService.GetPossibleRolesByType(type, roleId);

            // Check if it's role admin or any other.
            if (DatabaseConstants.Role.Admin.Id == user.RoleId)
                users = await _repositoryContext.Users.Where(u => !u.Deleted && availableRoles.Any(r => r == u.RoleId)).AsNoTracking().ToListAsync();
            else
                users = await GetAllUserChild(user.Id);

            var userIds = users.Where(u => availableRoles.Any(r => r == u.RoleId)).Select(u => u.Id).ToList();

            return userIds;
        }

        public async Task<List<int>> GetAllAdminIds() => await _repositoryContext.Users.Where(u => u.Deleted == false &&
        u.StatusId != DatabaseConstants.Status.Inactive.Id && u.RoleId == DatabaseConstants.Role.Admin.Id).Select(u => u.Id).ToListAsync();

        public async Task<int> GetUserId(string email) =>
            await _repositoryContext.Users.Where(u => u.Email.Equals(email) && u.Deleted == false).Select(u => u.Id).FirstOrDefaultAsync();

        public async Task<int?> GetUserRoleId(int userId) => await _repositoryContext.Users
            .Where(u => u.Id == userId && u.Deleted == false).Select(u => u.RoleId).FirstOrDefaultAsync();

        public async Task<User?> GetUser(string email, string password)
        {
            // Get user
            User? user = await _repositoryContext.Users.Where(u => u.Email.Equals(email) && u.StatusId != DatabaseConstants.Status.Inactive.Id && u.Deleted == false)
                .Include(u => u.Role).FirstOrDefaultAsync();

            // If user is not found return null
            if (user == null)
                return null;

            var password1 = password.ToMD5Hash();

            // If password matches in SHA1 change it to SHA512 and return the user
            if (user.Password == password.ToMD5Hash())
            {
                user.Password = password.ToSHA512Hash();
                await _repositoryContext.SaveChangesAsync();
                return user;
            }
            // If password matches in SHA512 return the user
            if (user.Password == password.ToSHA512Hash())
            {
                return user;
            }

            return null;
        }

        public async Task<User?> GetUser(string email, bool trackChanges = true) =>
            await FindByCondition(u => u.Deleted == false && u.Email.Equals(email), trackChanges)
            .Include(u => u.OneTimeTokens)
            .Include(u => u.Role).FirstOrDefaultAsync();

        public async Task<User?> GetUser(int userId, bool trackChanges = true) =>
            await FindByCondition(u => u.Deleted == false && u.Id.Equals(userId), trackChanges)
            .Include(u => u.OneTimeTokens).Include(u => u.Country).Include(u => u.Language)
            .Include(u => u.Role).Include(u => u.Status).Include(u => u.Parent).FirstOrDefaultAsync();

        public async Task<User?> GetUserForFullUpdate(int userId) => await _repositoryContext.Users
            .Where(u => u.Deleted == false && u.Id.Equals(userId)).FirstOrDefaultAsync();


        public async Task<TableResponse<User>> GetUsersPaginated(DatatableOptions options, bool includeInactive, bool isAdmin, int loggedInUserId, bool shortVersion)
        {
            IQueryable<User> users;
            if (isAdmin)
            {
                // If it's admin get all users
                users = _repositoryContext.Users.Include(u => u.Role).Include(u => u.Status).Include(u => u.Country).Include(u => u.Language).Include(u => u.Parent)
                    .Where(u => (EF.Functions.Like(u.Name.ToString(), $"%{options.Search}%") || EF.Functions.Like(u.Email.ToString(), $"%{options.Search}%")) && u.Deleted == false);
            }
            else
            {
                // If user is not an admin get child users
                List<int> userIds = await GetAllUserChildIds(loggedInUserId);
                users = _repositoryContext.Users.Include(u => u.Role).Include(u => u.Status).Include(u => u.Country).Include(u => u.Language).Include(u => u.Parent)
                    .Where(u => (EF.Functions.Like(u.Name.ToString(), $"%{options.Search}%") || EF.Functions.Like(u.Email.ToString(), $"%{options.Search}%"))
                    && userIds.Any(id => id == u.Id) && u.Deleted == false);
            }

            if (!includeInactive)
            {
                // Exclude inactive
                users = users.Where(u => u.StatusId != DatabaseConstants.Status.Inactive.Id);
            }

            if (shortVersion)
            {
                // Convert to short version
                users = users.Select(u => new User
                {
                    Id = u.Id,
                    Name = u.Name,
                });

                options.SortBy = "Name";
                options.Page = 1;
                options.PerPage = await users.CountAsync();
            }
            else
            {
                // Include parents if it's a long version
                users.Include(u => u.Parent);
            }

            // Count and adjust
            options.TotalRecords = await users.CountAsync();

            return new TableResponse<User>
            {
                Responses = await users.OrderByPage(options.SortBy, options.SortByDirection, options.Page, options.PerPage).ToListAsync(),
                Options = options
            };
        }

        public async Task<List<User>> GetUsers(List<int> userIds, bool trackChanges = true)
        {
            return await FindAll(trackChanges)
                    .Include(u => u.Role)
                    .Include(u => u.Status)
                    .Include(u => u.Country)
                    .Include(u => u.Parent)
                      .Where(u => userIds.Contains(u.Id))
                      .ToListAsync();
        }

        public async Task<List<int>> GetPossibleUserIds(bool isAdmin, int loggedUserId, bool trackChanges = true)
        {
            IQueryable<int> query = _repositoryContext.Users
                    .Where(u => u.Deleted == false)
                    .Select(u => u.Id);

            if (!isAdmin)
            {
                // If user is not an admin, filter by child user IDs
                List<int> childIds = await GetAllUserChildIds(loggedUserId);
                query = query.Where(id => childIds.Contains(id));
            }

            return await query.ToListAsync();
        }

        #endregion

        #region Update
        public async Task UpdateUser(User user)
        {
            // Set user to modified and save changes
            _repositoryContext.Entry(user).State = EntityState.Modified;
            await _repositoryContext.SaveChangesAsync();
        }

        public async Task<List<User>> UpdateAListOfUsers(List<User> users)
        {
            // Update a list of users
            UpdateList(users);

            await _repositoryContext.SaveChangesAsync();

            // Return updated users
            return users;
        }


        #endregion

        #region Create
        public async Task CreateUser(User user)
        {
            // Create user
            Create(user);
            await _repositoryContext.SaveChangesAsync();
        }

        public async Task<List<User>> CreateAListOfUsers(List<User> users)
        {
            // Create a list of users
            CreateList(users);
            await _repositoryContext.SaveChangesAsync();

            // Return created users
            return users;
        }

        #endregion

        #region Delete

        public async Task DeleteAllChildren(int userId)
        {
            // Get users that have parentId as parentId
            List<User> childUsers = await _repositoryContext.Users.Where(u => u.ParentId == userId).ToListAsync();
            foreach (var childUser in childUsers)
            {
                childUser.Deleted = true;
                await DeleteAllChildren(childUser.Id);
            }

            await _repositoryContext.SaveChangesAsync();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Get all user child
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<List<int>> GetAllUserChildIds(int userId)
        {
            // Get all child users for selected user
            var users = await _repositoryContext.Users
                .Where(u => u.ParentId == userId && !u.Deleted).Select(u => u.Id).ToListAsync();

            // Get all users child
            foreach (var user in users.ToList())
                users.AddRange(await GetAllUserChildIds(user));

            users.Add(userId);

            return users;
        }

        /// <summary>
        /// Get a list of user children as an object list
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="includeCurrentUser"></param>
        /// <returns></returns>
        private async Task<List<User>> GetAllUserChild(int userId, bool includeCurrentUser = true)
        {
            // Get all child users for selected user
            var users = await _repositoryContext.Users
                .Where(u => u.ParentId == userId && !u.Deleted && u.StatusId != DatabaseConstants.Status.Inactive.Id).AsNoTracking().ToListAsync();

            // Get all users child
            foreach (var user in users.ToList())
                users.AddRange(await GetAllUserChild(user.Id, false));

            // Add current user
            if (includeCurrentUser)
                users.Add(await _repositoryContext.Users.AsNoTracking()
                        .FirstAsync(u => u.Id == userId));

            return users;
        }

        #endregion
    }
}
