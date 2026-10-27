using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class UnitImageRepository : RepositoryBase<UnitImage>, IUnitImageRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public UnitImageRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task CreateUnitImage(UnitImage newUnitImage)
        {
            Create(newUnitImage);

            await _repositoryContext.SaveChangesAsync();
        }

        public async Task UpdateUnitImage(UnitImage newUnitImage)
        {
            Update(newUnitImage);

            await _repositoryContext.SaveChangesAsync();
        }

        public async Task<UnitImage?> GetUnitInstallationImage(int unitId) => await _repositoryContext.UnitImages
            .Where(i => i.UnitId.Equals(unitId) && i.UnitCategoryId == DatabaseConstants.UnitCategory.Install.Id).FirstOrDefaultAsync();
    }
}
