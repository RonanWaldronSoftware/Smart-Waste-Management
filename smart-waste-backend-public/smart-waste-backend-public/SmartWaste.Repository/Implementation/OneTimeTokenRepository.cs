using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public sealed class OneTimeTokenRepository : RepositoryBase<OneTimeToken>, IOneTimeTokenRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public OneTimeTokenRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<bool> CheckOneTimeToken(string token) => await _repositoryContext.OneTimeTokens.AnyAsync(t => t.Token.ToString() == token);

        public async Task<OneTimeToken?> GetOneTimeToken(string token) => await _repositoryContext.OneTimeTokens.AsNoTracking().Where(t => t.Token.ToString() == token).FirstOrDefaultAsync();

        public async Task<List<OneTimeToken>> GetOldOneTimeTokens(int numberOfDaysForTokenDelete) => 
            await _repositoryContext.OneTimeTokens.Where(t => t.CreateDate <= DateTime.UtcNow.AddDays(-numberOfDaysForTokenDelete)).ToListAsync();

        public async Task<OneTimeToken> CreateOneTimeToken(int userId)
        {
            // Create new local one time token
            OneTimeToken passwordResetToken = new OneTimeToken()
            {
                UserId = userId,
                Token = Guid.NewGuid(),
            };

            // Add one time token to database
            Create(passwordResetToken);
            await _repositoryContext.SaveChangesAsync();

            // Return new one time token
            return passwordResetToken;
        }

        public async Task DeleteOneTimeToken(string token)
        {
            // Delete one time token
            var oneTimeToken = await _repositoryContext.OneTimeTokens
                .Where(t => t.Token.ToString() == token).FirstOrDefaultAsync();

            if (oneTimeToken != null)
            {
                Delete(oneTimeToken);
            }

            // Save changes
            await _repositoryContext.SaveChangesAsync();
        }

        public async Task DeleteOneTimeTokenList(List<OneTimeToken> oneTimeTokens)
        {
            // Delete a list of one time tokens
            DeleteList(oneTimeTokens);

            // Save changes
            await _repositoryContext.SaveChangesAsync();
        }
    }
}
