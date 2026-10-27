using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public RoleRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<List<Role>> GetRoles() => await _repositoryContext.Roles.ToListAsync();
    }
}
