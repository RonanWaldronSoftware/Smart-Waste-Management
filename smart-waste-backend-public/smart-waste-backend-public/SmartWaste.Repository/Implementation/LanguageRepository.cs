using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class LanguageRepository : RepositoryBase<Language>, ILanguageRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public LanguageRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<Language> GetLanguage(int languageId, bool trackChanges = true)
            => await FindByCondition(l => l.Id == languageId, trackChanges).FirstAsync();

        public async Task<List<Language>> GetLanguages() => await _repositoryContext.Languages.ToListAsync();
    }
}
