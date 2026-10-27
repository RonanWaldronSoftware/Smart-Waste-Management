using SmartWaste.Model.Core;
using SmartWaste.Model.Enums;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class AlertTypeRepository : RepositoryBase<AlertType>, IAlertTypeRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public AlertTypeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<List<AlertType>> GetAlertTypes() => await _repositoryContext.AlertTypes.ToListAsync();

        public async Task<List<AlertType>> GetAlertTypes(AlertTypeCategoryEnum categoryEnum, bool trackChanges = true)
        => await FindByCondition(a => a.AlertTypeCategoryId == (int)categoryEnum, trackChanges).ToListAsync();

        public async Task<List<AlertType>> GetAlertsWithUser(List<int> childUserIds) => await _repositoryContext.AlertTypes
            .Include(a => a.UserAlerts.Where(ua => ua.User.Deleted == false && childUserIds.Any(ch => ch == ua.UserId)))
            .ThenInclude(ua => ua.User).ToListAsync();

        public async Task<AlertType> GetAlertType(int alertTypeId, bool trackChanges = true)
        => await FindByCondition(at => at.Id == alertTypeId,trackChanges).FirstOrDefaultAsync();
    }
}
