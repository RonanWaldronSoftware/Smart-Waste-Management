using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface ILanguageRepository
    {
        /// <summary>
        /// Get a language by Id
        /// </summary>
        /// <param name="languageId"></param>
        /// <returns></returns>
        public Task<Language> GetLanguage(int languageId, bool trackChanges = true);

        /// <summary>
        /// Get all languages
        /// </summary>
        /// <returns></returns>
        public Task<List<Language>> GetLanguages();
    }
}
