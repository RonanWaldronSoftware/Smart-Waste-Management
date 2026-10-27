using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class UnitTypeRepository : RepositoryBase<UnitType>, IUnitTypeRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public UnitTypeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<List<UnitType>> GetUnitTypes() => await _repositoryContext.UnitTypes.ToListAsync();
    }
}
