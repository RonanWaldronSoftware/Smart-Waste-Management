using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IDeviceInputTypeRepository
    {
        /// <summary>
        /// Get all device input types
        /// </summary>
        /// <returns></returns>
        public Task<List<DeviceInputType>> GetDeviceInputTypes();
    }
}