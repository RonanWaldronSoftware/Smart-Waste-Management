using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class DeviceTypeRepository : RepositoryBase<DeviceType>, IDeviceTypeRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public DeviceTypeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<List<DeviceType>> GetDeviceTypes() => await _repositoryContext.DeviceTypes.ToListAsync();
    }
}
