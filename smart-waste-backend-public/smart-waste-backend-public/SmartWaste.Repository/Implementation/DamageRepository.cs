using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Model.Extensions;
using SmartWaste.Model.Helpers;
using SmartWaste.Model.JsonModels.Damage;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class DamageRepository : RepositoryBase<Damage>, IDamageRepository
    {
        private readonly RepositoryContext _repositoryContext;
        public DamageRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task CreateDamage(CreateDamageInput input, int unitId, string uploadedImage, int userId)
        {
            // Create new damage
            var damage = new Damage()
            {
                DamageTypeId = input.DamageTypeId,
                StatusId = DatabaseConstants.DamageStatus.Open.Id,
                UnitId = unitId,
            };

            // Create damage history
            var damageHistory = new DamageHistory
            {
                UserId = userId,
                StatusId = damage.StatusId,
                Image = uploadedImage,
                Comment = input.Comment,
            };

            damage.DamageHistories.Add(damageHistory);

            Create(damage);

            await _repositoryContext.SaveChangesAsync();
        }

        public async Task<TableResponse<Damage>> GetActiveDamages(DatatableOptions options, List<int> possiblePerentIds, bool trackChanges = true)
        {
            var damages = FindAll(trackChanges)
                .Include(d => d.Unit).ThenInclude(u => u.UnitModelType)
                .Include(d => d.DamageType)
                .Include(d => d.Status)
                .Include(d => d.DamageHistories).ThenInclude(dh => dh.User)
                .Include(d => d.DamageHistories).ThenInclude(dh => dh.AssignedUser)
                .Where(d => possiblePerentIds.Any(id => id == d.Unit.UserId) && d.StatusId != DatabaseConstants.DamageStatus.Closed.Id && (EF.Functions.Like(d.Unit.Location, $"%{options.Search}%")
                            || EF.Functions.Like(d.DamageHistories.OrderBy(dh => dh.CreateDate).FirstOrDefault().User.Name, $"%{options.Search}%")
                            || EF.Functions.Like(d.DamageHistories.OrderByDescending(dh => dh.CreateDate).FirstOrDefault().AssignedUser.Name, $"%{options.Search}%")
                            || EF.Functions.Like(d.DamageType.Name, $"%{options.Search}%")
                            || EF.Functions.Like(d.Unit.UnitModelType.Name, $"%{options.Search}%")));

            options.TotalRecords = await damages.CountAsync();

            return new TableResponse<Damage>
            {
                Responses = await damages.OrderByPage(options.SortBy, options.SortByDirection, options.Page, options.PerPage).ToListAsync(),
                Options = options
            };
        }

        public async Task<Damage?> GetDamage(int damageId, bool trackChanges = true)
        => await FindByCondition(d => d.Id == damageId, trackChanges)
            .Include(d => d.Unit)
            .Include(d => d.DamageHistories)
            .FirstOrDefaultAsync();

        public async Task<int> GetNavigationCount(List<int> possibleParentsIds, bool trackChanges = true)
        {
            return await FindAll(trackChanges).Where(d => possibleParentsIds.Contains(d.Unit.UserId.Value) && (d.StatusId == DatabaseConstants.DamageStatus.Open.Id
                                                || d.StatusId == DatabaseConstants.DamageStatus.Assigned.Id)).CountAsync();
        }

        public async Task<List<Damage>> GetUnitDamages(int unitId, bool trackChanges = true)
         => await FindAll(trackChanges).Where(d => d.UnitId == unitId)
                    .Include(d => d.DamageHistories.OrderByDescending(dh => dh.CreateDate)).ThenInclude(dh => dh.AssignedUser)
                    .Include(d => d.DamageHistories).ThenInclude(dh => dh.User)
                    .Include(d => d.DamageType)
                    .ToListAsync();
    }
}
