using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface ICountryRepository
    {
        /// <summary>
        /// Get all countries
        /// </summary>
        /// <returns></returns>
        public Task<List<Country>> GetCountries();
    }
}
