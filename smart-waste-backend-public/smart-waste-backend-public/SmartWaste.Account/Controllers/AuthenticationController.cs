using SmartWaste.Account.AppSettings;
using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Model.Error;
using SmartWaste.Model.Extensions;
using SmartWaste.Model.JsonModels.Account;
using SmartWaste.Repository;
using SmartWaste.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SmartWaste.Model.Helpers.Jwt;
using Newtonsoft.Json;
using System.Security.Claims;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Helpers.RabbitMQ;
using SmartWaste.Model.Resources;
namespace SmartWaste.Account.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IServiceManager _serviceManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly LocalizerService _localizer;

        public AuthenticationController(ILogger<AuthenticationController> logger, IServiceManager serviceManager,
            IRepositoryManager repositoryManager, IJwtAuthManager jwtAuthManager, IHttpContextAccessor accessor, LocalizerService localizer)
        {
            _logger = logger;
            _serviceManager = serviceManager;
            _repositoryManager = repositoryManager;
            _jwtAuthManager = jwtAuthManager;
            _localizer = localizer;

            // Add ip address and browser in scope
            _logger.BeginScope(new Dictionary<string, object>
            {
                ["ip_address"] = accessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                ["browser"] = accessor.HttpContext.Request.Headers.UserAgent.ToString()
            });
        }

        [HttpPost("Login")]
        [SwaggerOperation(summary: "Login user", description: "Login with email and password")]
        [SwaggerResponse(200, "User data", typeof(JwtAuthResult))]
        [SwaggerResponse(403)]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AccountLoginInput model)
        {
            // Get user
            var user = await _repositoryManager.User.GetUser(model.Email, model.Password);

            // If it's a valid user generate and return a new jwt token
            if (user is not null && user.StatusId != DatabaseConstants.Status.Inactive.Id)
            {
                // Check if user send us push notification token
                if (!string.IsNullOrEmpty(model.DeviceToken))
                    await _repositoryManager.User.AddDeviceToken(user.Id, model.DeviceToken, model?.DeviceType ?? 1);

                _logger.LogInformation($"Login success for user {user.Email}");
                return Ok(await GetUserAccessToken(user));
            }

            _logger.LogInformation($"Login for {model.Email} and provided password failed");
            return Forbid();
        }

        [HttpGet("Logout")]
        [SwaggerOperation(summary: "Logout user", description: "Logout user from application")]
        [SwaggerResponse(200, "Logged out")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Remove all user tokens
            await _repositoryManager.User.RemoveUserTokens(User.Identity?.Name ?? string.Empty);

            return Ok();
        }

        [HttpPost("RefreshToken")]
        [SwaggerOperation(summary: "Refresh token", description: "Refresh access token with refresh token and get new extended access token")]
        [SwaggerResponse(200, "User data", typeof(JwtAuthResult))]
        [SwaggerResponse(400, "Token is not recognized", typeof(HttpError))]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenInput model)
        {
            try
            {
                using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(model))))
                {
                    _logger.LogInformation("User want to refresh token");
                }

                // Generate and return a new jwt token
                var email = _jwtAuthManager.Refresh(model.RefreshToken, model.AccessToken);

                // Get user data
                var user = await _repositoryManager.User.GetUser(email);

                // Check if user exist
                if (user == null) return BadRequest(new HttpError("Token is not recognized"));

                _logger.LogInformation($"Login success with refresh token for user {user.Email}");
                return Ok(await GetUserAccessToken(user));
            }
            catch (Exception ex)
            {
                using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(ex.ToString()))))
                {
                    _logger.LogError("An error occurred while refreshing token");
                }
                return BadRequest(new HttpError("Token is not recognized"));
            }
        }

        [HttpPost("ForgotPassword")]
        [SwaggerOperation(summary: "Forgot password", description: "Send email with link for resetting password")]
        [SwaggerResponse(200, "Send password reset email")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordInput model)
        {
            // Get user id
            _logger.LogInformation($"{model.Email} requesting password reset");

            // Check if email exist. If not exist return not found
            if (!await _repositoryManager.User.IsUserEmailExist(model.Email))
            {
                _logger.LogWarning($"{model.Email} does not exist in database");
                return Ok();
            }

            // Create one time token for user
            var newOneTimeToken = await _serviceManager.AccountService.CreateOneTimeToken(model.Email);

            // Send email to queue
            _logger.LogInformation($"Password reset mail for {model.Email} going to queue");

            // Create notificatin model and add send model to rabbit mq
            QueueNotificationActionModel notificationModel = new QueueNotificationActionModel()
            {
                Email = new EmailRabbitMqMessage
                {
                    SendTo = model.Email,
                    Template = EmailTemplateEnum.PasswordReset,
                    Subject = "Password reset",
                    User = await _repositoryManager.User.GetUser(model.Email, false),
                },
            };

            _serviceManager.RabbitMQService.SendMessage(notificationModel, GlobalConstants.NotificationQueue);

            return Ok();
        }

        [HttpGet("CheckToken")]
        [SwaggerOperation(summary: "Check token", description: "Check token for forgot password to check if is correct")]
        [SwaggerResponse(200, "Token is correct")]
        [SwaggerResponse(400, "Token not found", typeof(HttpError))]
        [AllowAnonymous]
        public async Task<IActionResult> CheckToken(string token)
        {
            _logger.LogInformation($"Checking token: {token}");
            // Check if one time token exists
            return await _repositoryManager.OneTimeToken.CheckOneTimeToken(token) ? Ok() : BadRequest(new HttpError($"Token {token} does not exist"));
        }

        [HttpPost("PasswordReset")]
        [SwaggerOperation(summary: "Password reset", description: "Change password for user with specific token")]
        [SwaggerResponse(200, "Change password", typeof(JwtAuthResult))]
        [SwaggerResponse(400, "Token does not exist", typeof(HttpError))]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotPasswordChangePasswordInput model)
        {
            // Get one time token
            var oneTimeToken = await _repositoryManager.OneTimeToken.GetOneTimeToken(model.Token);

            if (oneTimeToken is null)
            {
                _logger.LogWarning($"Password reset token {model.Token} was not found");
                return BadRequest(new HttpError($"Token not found"));
            }

            // Get user
            var user = await _repositoryManager.User.GetUser(oneTimeToken.UserId);

            // Check if user exist
            if (user is not null)
            {
                // Set user password and status
                user.Password = model.Password.ToSHA512Hash();
                user.StatusId = DatabaseConstants.Status.Active.Id;

                // Update password
                await _repositoryManager.User.UpdateUser(user);
                _logger.LogInformation($"Reset password via forgot password User: {user.Email}");

                // Delete one time token
                await _repositoryManager.OneTimeToken.DeleteOneTimeToken(model.Token);
                _logger.LogInformation($"Deleted OneTimeToken for User: {user.Email}");

                // Return tokens
                return Ok(_jwtAuthManager.GenerateTokens(user));
            }

            _logger.LogWarning($"User for token {model.Token} not found");
            return BadRequest(new HttpError("User is deleted or provided data are wrong"));
        }

        [HttpPost("ChangePassword")]
        [SwaggerOperation(summary: "Change user password", description: "Change user password and set status registered")]
        [SwaggerResponse(200, "Change password", typeof(JwtAuthResult))]
        [SwaggerResponse(400, "Could not change user password", typeof(HttpError))]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string email, string password)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            // Get language
            Language language = await _repositoryManager.Language.GetLanguage(languageId);

            // Get logged in user
            var loggedInUser = await _repositoryManager.User.GetUser(loggedInUserId, false);

            if (loggedInUser is null)
            {
                _logger.LogWarning($"User {loggedInUserId} can't change password for {email}");
                return BadRequest(new HttpError(String.Format(_localizer.GetLocalString("changePasswordErrorResponse", language.Code), loggedInUserId)));
            }

            // Get user
            var user = await _repositoryManager.User.GetUser(email);

            if (user is not null)
            {
                try
                {
                    // Check if user is admin or is a parent of a user (checked recursively for all child-parent relationships)
                    if (loggedInUser.RoleId == DatabaseConstants.Role.Admin.Id || await _repositoryManager.User.CanUserChangeData(loggedInUser.Id, user.Id))
                    {
                        _logger.LogInformation($"UserId: {loggedInUser.Id}, changing password for userId: {user.Id}");

                        // Update password
                        user.Password = password.ToSHA512Hash();
                        user.StatusId = DatabaseConstants.Status.Registered.Id;

                        await _repositoryManager.User.UpdateUser(user);
                        _logger.LogInformation($"Set new password for user: {user.Id} by user: {loggedInUser.Id}");

                        return Ok();
                    }
                }
                catch (Exception ex)
                {
                    using (_logger.BeginScope(("full_message", JsonConvert.SerializeObject(ex.ToString()))))
                    {
                        _logger.LogError($"An error occurred when user {loggedInUserId} tried changing password for {email}");
                    }
                }
            }
            return BadRequest(new HttpError(_localizer.GetLocalString("changePasswordAuthErrorMessage", language.Code)));
        }

        #region Private methods

        /// <summary>
        /// Generate user access token depend on status
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<AccountLoginResponse> GetUserAccessToken(User user)
        {
            var response = new AccountLoginResponse();

            if (user.StatusId == DatabaseConstants.Status.Registered.Id)
            {
                // Create one time token for user
                var newOneTimeToken = await _serviceManager.AccountService.CreateOneTimeToken(user.Email ?? "");
                response.OneTimeToken = newOneTimeToken.Token.ToString();
            }
            else if (user.StatusId == DatabaseConstants.Status.Active.Id)
                response.JwtAuthResult = _jwtAuthManager.GenerateTokens(user);

            return response;
        }

        #endregion
    }
}
