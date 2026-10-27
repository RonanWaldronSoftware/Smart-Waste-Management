using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class StatusCategoryRepository : RepositoryBase<Status>, IStatusCategoryRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public StatusCategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<List<StatusCategory>> GetStatusCategories() => await _repositoryContext.StatusCategories.ToListAsync();
    }
}
