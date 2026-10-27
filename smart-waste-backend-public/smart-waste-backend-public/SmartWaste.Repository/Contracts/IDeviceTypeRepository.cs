using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IDeviceTypeRepository
    {
        /// <summary>
        /// Get all device types
        /// </summary>
        /// <returns></returns>
        public Task<List<DeviceType>> GetDeviceTypes();
    }
}
