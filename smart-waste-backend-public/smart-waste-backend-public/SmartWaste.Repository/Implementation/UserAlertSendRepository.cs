using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    internal class UserAlertSendRepository : RepositoryBase<UserAlertSend>, IUserAlertSendRepository
    {
        private readonly RepositoryContext _repositoryContext;
        public UserAlertSendRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task CreateUserSendAlert(UserAlertSend userAlertSend)
        {
            Create(userAlertSend);
            await _repositoryContext.SaveChangesAsync();
        }

        public async Task DeleteUserAlertsSendExcludingTypes(List<AlertType> excludingAlertTypes, int userId, int unitId)
        {
            // Get types ids
            var alertTypeIds = excludingAlertTypes.Select(at => at.Id).ToList();

            var userSendAlertsForDelete = await _repositoryContext.UserAlertSends
                .Include(uas => uas.UserAlert)
                .Where(uas => !alertTypeIds.Contains(uas.UserAlert.AlertTypeId.Value) && uas.UserAlert.UserId == userId && uas.UnitId == unitId)
                .ToListAsync();

            _repositoryContext.UserAlertSends.RemoveRange(userSendAlertsForDelete);
            await _repositoryContext.SaveChangesAsync();
        }
    }
}
