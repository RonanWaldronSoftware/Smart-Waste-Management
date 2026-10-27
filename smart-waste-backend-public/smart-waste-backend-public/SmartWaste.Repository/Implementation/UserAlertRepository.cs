using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using LinqKit;
using System.Linq;

namespace SmartWaste.Repository.Implementation
{
    public sealed class UserAlertRepository : RepositoryBase<UserAlert>, IUserAlertRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public UserAlertRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<List<UserAlert>?> GetUserAlerts(List<(int userId, int customerUserId)> userUserCustomerIds, bool trackChanges = true)
        {
            // Ensure there are pairs to process
            if (userUserCustomerIds == null || !userUserCustomerIds.Any())
            {
                return new List<UserAlert>();
            }

            var predicate = PredicateBuilder.New<UserAlert>();

            foreach (var pair in userUserCustomerIds)
            {
                var tempPair = pair; // Prevent modified closure issue
                predicate = predicate.Or(ua => ua.UserId == tempPair.userId && ua.CustomerUserId == tempPair.customerUserId);
            }

            var userAlertsQuery = _repositoryContext.UserAlerts
                .Include(ua => ua.User).ThenInclude(u => u.Parent)
                .Include(ua => ua.AlertType)
                .Include(ua => ua.CustomerUser)
                .Where(predicate);

            return await userAlertsQuery.ToListAsync();
        }

        public async Task CreateUserAlerts(List<UserAlert> newUserAlert)
        {
            CreateList(newUserAlert);

            await _repositoryContext.SaveChangesAsync();
        }

        public async Task DeleteAlerts(List<UserAlert> userAlerts)
        {
            var userAlertIds = userAlerts.Select(ua => ua.Id).ToList();

            // Delete user alert sends
            var userAlertSends = await _repositoryContext.UserAlertSends
                .Where(uas => userAlertIds.Contains(uas.UserAlertId.Value))
                .ToListAsync();

            _repositoryContext.UserAlertSends.RemoveRange(userAlertSends);

            DeleteList(userAlerts);
            await _repositoryContext.SaveChangesAsync();
        }
    }
}
