using SmartWaste.Model.JsonModels.Account;

namespace SmartWaste.Service.Contracts
{
    public interface IConfigurationService
    {
        /// <summary>
        /// Get configurations
        /// </summary>
        /// <returns></returns>
        public Task<WebConfigurationResponse> GetConfiguration(int languageId, int userRole);
    }
}
