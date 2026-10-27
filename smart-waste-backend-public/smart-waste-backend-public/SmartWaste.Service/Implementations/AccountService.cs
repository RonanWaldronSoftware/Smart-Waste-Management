using SmartWaste.Model.Core;
using SmartWaste.Repository;
using SmartWaste.Service.Contracts;
using Microsoft.Extensions.Options;
using SmartWaste.Model.Configuration;

namespace SmartWaste.Service.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly Configuration _configuration;
        private int _numberOfDaysForOneTimeTokenDelete;
        public AccountService(IOptions<Configuration> configuration, IRepositoryManager repositoryManager) 
        {
            _configuration = configuration.Value;
            _numberOfDaysForOneTimeTokenDelete = _configuration.NumberOfDaysForOneTimeTokenDelete;
            _repositoryManager = repositoryManager;
        }

        public async Task<OneTimeToken> CreateOneTimeToken(string email)
        {
            // Get user Id for email
            int userId = await _repositoryManager.User.GetUserId(email);

            // Create one time token
            return await _repositoryManager.OneTimeToken.CreateOneTimeToken(userId);
        }

        public async Task DeleteOldOneTimeTokens()
        {
            // Get old one time tokens
            List<OneTimeToken> oldTokens = await _repositoryManager.OneTimeToken.GetOldOneTimeTokens(_numberOfDaysForOneTimeTokenDelete);

            // Delete old one time tokens
            await _repositoryManager.OneTimeToken.DeleteOneTimeTokenList(oldTokens);
        }
    }
}
