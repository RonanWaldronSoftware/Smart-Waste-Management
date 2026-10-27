using SmartWaste.Model.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text;
using SmartWaste.Model.Configuration;
using SmartWaste.Model.Helpers.Jwt;

namespace SmartWaste.Account.AppSettings
{
    public interface IJwtAuthManager
    {
        /// <summary>
        /// Refreshed tokens from user
        /// </summary>
        IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary { get; }

        /// <summary>
        /// Generate new access token for user with specific claims and username
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        JwtAuthResult GenerateTokens(User user);
        
        /// <summary>
        /// Refresh user access token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        string Refresh(string refreshToken, string accessToken);

        /// <summary>
        /// Remove all refreshed tokens for specific usernames
        /// </summary>
        /// <param name="userName"></param>
        void RemoveRefreshTokenByUserName(string userName);
    }

    public class JwtAuthManager : IJwtAuthManager
    {
        public IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary => _usersRefreshTokens.ToImmutableDictionary();
        private readonly ConcurrentDictionary<string, RefreshToken> _usersRefreshTokens;
        private readonly IOptions<JwtConfiguration> _jwtConfiguration;
        private readonly byte[] _secret;

        public JwtAuthManager(IOptions<JwtConfiguration> jwtConfiguration)
        {
            _jwtConfiguration = jwtConfiguration;
            _usersRefreshTokens = new ConcurrentDictionary<string, RefreshToken>();
            _secret = Encoding.ASCII.GetBytes(jwtConfiguration.Value.Secret);
        }

        public void RemoveRefreshTokenByUserName(string userName)
        {
            // Get refreshed token only specific for that username
            var refreshTokens = _usersRefreshTokens.Where(x => (x.Value.UserName ?? "").Equals(userName)).ToList();

            // Remove refreshed token
            foreach (var refreshToken in refreshTokens)
            {
                _usersRefreshTokens.TryRemove(refreshToken.Key, out _);
            }
        }

        public JwtAuthResult GenerateTokens(User user)
        {
            List<Claim> claims = GetUserClaims(user);
            // Check if claim is empty
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);

            // Generate new jwt security
            var jwtToken = new JwtSecurityToken(
                _jwtConfiguration.Value.ValidIssuer,
                shouldAddAudienceClaim ? _jwtConfiguration.Value.ValidAudience : string.Empty,
                claims,
                expires: DateTime.Now.AddHours(_jwtConfiguration.Value.AccessTokenExpiration),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));

            // Generate new access token
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            // Create refresh token
            var refreshToken = new RefreshToken
            {
                UserName = user.Email,
                TokenString = GenerateRefreshTokenString(),
                ExpireAt = DateTime.Now.AddHours(_jwtConfiguration.Value.RefreshTokenExpiration)
            };

            // Add refresh token into list
            _usersRefreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (_, _) => refreshToken);

            // Return refresh and access token
            return new JwtAuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        
        public string Refresh(string refreshToken, string accessToken)
        {
            // Decode current access token
            var (principal, jwtToken) = DecodeJwtToken(accessToken);

            // Check if token is correct
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                throw new SecurityTokenException("Invalid token");
            }

            // Get username from current user
            var userName = principal.Identity?.Name;

            // Check if token is correct
            if (!_usersRefreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
            {
                throw new SecurityTokenException("Invalid token");
            }

            // Check if username in token and username for refresh is ok
            if (existingRefreshToken.UserName != userName || existingRefreshToken.ExpireAt < DateTime.Now)
            {
                throw new SecurityTokenException("Invalid token");
            }

            return userName;
        }

        /// <summary>
        /// Decode current jwt token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
        {
            // Check if token is set
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }

            // Decode current token
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = _jwtConfiguration.Value.ValidIssuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(_secret),
                        ValidAudience = _jwtConfiguration.Value.ValidAudience,
                        ValidateAudience = true,
                        ValidateLifetime = false,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    },
                    out var validatedToken);

            return (principal, validatedToken as JwtSecurityToken);
        }

        /// <summary>
        /// Generate new refresh token
        /// </summary>
        /// <returns></returns>
        private static string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private List<Claim> GetUserClaims(User user)
        {
            // Get user claims
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Id.ToString()),
                new Claim("LanguageId",user.LanguageId.ToString()),

            };

            claims.AddRange(_jwtConfiguration.Value.DefaultScope.Select(scope => new Claim("scope", scope)));

            return claims;
        }
    }
}
