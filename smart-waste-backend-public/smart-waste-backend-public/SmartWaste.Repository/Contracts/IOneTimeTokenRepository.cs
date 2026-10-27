using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IOneTimeTokenRepository
    {
        /// <summary>
        /// Chekc if token exists in one time tokens
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<bool> CheckOneTimeToken(string token);

        /// <summary>
        /// Get one time token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<OneTimeToken?> GetOneTimeToken(string token);

        /// <summary>
        /// Get old one time tokens
        /// </summary>
        /// <returns></returns>
        public Task<List<OneTimeToken>> GetOldOneTimeTokens(int numberOfDaysForTokenDelete);

        /// <summary>
        /// Create one time token
        /// </summary>
        /// <param name="newOneTimeToken"></param>
        /// <returns></returns>
        public Task<OneTimeToken> CreateOneTimeToken(int userId);

        /// <summary>
        /// Delete one time token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task DeleteOneTimeToken(string token);

        /// <summary>
        /// Delete list of one time tokens
        /// </summary>
        /// <param name="oneTimeTokens"></param>
        /// <returns></returns>
        public Task DeleteOneTimeTokenList(List<OneTimeToken> oneTimeTokens);
    }
}
