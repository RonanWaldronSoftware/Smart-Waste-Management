using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IRoleRepository
    {
        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns></returns>
        public Task<List<Role>> GetRoles();
    }
}
