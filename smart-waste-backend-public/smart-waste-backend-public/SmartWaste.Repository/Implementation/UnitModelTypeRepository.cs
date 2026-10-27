using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class UnitModelTypeRepository : RepositoryBase<UnitModelType>, IUnitModelTypeRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public UnitModelTypeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<List<UnitModelType>> GetUnitModelTypes() => await _repositoryContext.UnitModelTypes.ToListAsync();
    }
}
