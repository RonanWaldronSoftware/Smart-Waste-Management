using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class StatusRepository : RepositoryBase<Status>, IStatusRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public StatusRepository(RepositoryContext repositoryContext) : base(repositoryContext) 
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<List<Status>> GetStatuses() => await _repositoryContext.Statuses.ToListAsync();
    }
}
