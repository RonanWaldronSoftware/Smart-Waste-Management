using SmartWaste.Model.Core;
using SmartWaste.Model.Helpers;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class DamageHistoryRepository : RepositoryBase<DamageHistory>, IDamageHistoryRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public DamageHistoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<List<DamageHistory>> GetUnitDamageHistory(int unitId, bool trackChanges = true)
        => await FindAll(trackChanges)
            .Include(dh=> dh.Damage).ThenInclude(d=> d.DamageType)
            .Where(dh => dh.Damage.UnitId == unitId).ToListAsync();
    }
}
