using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class AlertTypeCategoryRepository : RepositoryBase<AlertTypeCategory>, IAlertTypeCategoryRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public AlertTypeCategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<List<AlertTypeCategory>> GetAlertTypeCategories() => await _repositoryContext.AlertTypeCategories.ToListAsync();
    }
}
