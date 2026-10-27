using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class DeviceInputTypeRepository : RepositoryBase<DeviceInputType>, IDeviceInputTypeRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public DeviceInputTypeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<List<DeviceInputType>> GetDeviceInputTypes() => await _repositoryContext.DeviceInputTypes.ToListAsync();
    }
}