using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class CountryRepository : RepositoryBase<Country>, ICountryRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public CountryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<List<Country>> GetCountries() => await _repositoryContext.Countries.ToListAsync();
    }
}
