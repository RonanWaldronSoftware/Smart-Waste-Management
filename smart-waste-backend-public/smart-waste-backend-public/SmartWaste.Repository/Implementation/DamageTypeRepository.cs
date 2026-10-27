using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class DamageTypeRepository : RepositoryBase<DamageType>, IDamageTypeRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public DamageTypeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<List<DamageType>> GetDamageTypes() => await _repositoryContext.DamageTypes.ToListAsync();
    }
}
