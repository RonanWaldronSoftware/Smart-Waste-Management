using SmartWaste.Model.Core;

namespace SmartWaste.Service.Contracts
{
    public interface IAccountService
    {
        /// <summary>
        /// Create one time token
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<OneTimeToken> CreateOneTimeToken(string email);

        /// <summary>
        /// Delete all old one time tokens
        /// </summary>
        /// <returns></returns>
        public Task DeleteOldOneTimeTokens();
    }
}
