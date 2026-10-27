using AutoMapper;
using SmartWaste.Account.AppSettings;
using SmartWaste.Model.Configuration;
using SmartWaste.Model.Constants;
using SmartWaste.Model.Core;
using SmartWaste.Model.DataTransferObjects;
using SmartWaste.Model.DataTransferObjects.Account;
using SmartWaste.Model.Enums;
using SmartWaste.Model.Error;
using SmartWaste.Model.Helpers.Jwt;
using SmartWaste.Model.JsonModels.Account;
using SmartWaste.Model.OldCore;
using SmartWaste.Model.Resources;
using SmartWaste.Repository;
using SmartWaste.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SmartWaste.Account.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IOptions<Configuration> _configuration;
        private readonly IServiceManager _serviceManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IMapper _mapper;
        private readonly LocalizerService _localizer;

        public UserController(ILogger<UserController> logger, IOptions<Configuration> configuration,
            IServiceManager serviceManager, IJwtAuthManager jwtAuthManager, IRepositoryManager repositoryManager,
            IHttpContextAccessor accessor, IMapper mapper, LocalizerService localizer)
        {
            _logger = logger;
            _serviceManager = serviceManager;
            _repositoryManager = repositoryManager;
            _configuration = configuration;
            _jwtAuthManager = jwtAuthManager;
            _mapper = mapper;
            _localizer = localizer;

            // Add ip address and browser in scope
            _logger.BeginScope(new Dictionary<string, object>
            {
                ["ip_address"] = accessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                ["browser"] = accessor.HttpContext.Request.Headers.UserAgent.ToString()
            });
        }

        [HttpPost]
        [Authorize(Policy = "AdminDistrubCorporateCustomerPolicy")]
        [SwaggerOperation(summary: "Create new user", description: "Create new user on platform")]
        [SwaggerResponse(200, "User created")]
        [SwaggerResponse(400, "User create failed")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserInput newUser)
        {
            var loggedInUserEmail = User?.Identity?.Name ?? "";
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            try
            {
                // Create a new user
                return (await _serviceManager.UserService.CreateUser(newUser, loggedInUserEmail, languageId)).GenerateResponse(this);
            }
            catch (Exception ex)
            {
                Language language = await _repositoryManager.Language.GetLanguage(languageId);
                return ex.GenerateBadRequest(_logger, this, $"User create failed", _localizer.GetLocalString("createUserErrorMessage", language.Code));
            }
        }

        [HttpPut]
        [Authorize(Policy = "AdminDistrubCorporateCustomerPolicy")]
        [SwaggerOperation(summary: "Update user", description: "Update user on platform")]
        [SwaggerResponse(200, "User updated")]
        [SwaggerResponse(400, "User update failed", typeof(HttpError))]
        public async Task<IActionResult> Update([FromBody] UpdateUserInput userUpdate)
        {
            // Get logged in user claims
            int loggedInUserRole = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.Role).Value);
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            try
            {
                // Update user and return response that shows if there were errors or not
                return (await _serviceManager.UserService.UpdateUser(userUpdate, loggedInUserId, loggedInUserRole, languageId)).GenerateResponse(this);
            }
            catch (Exception ex)
            {
                Language language = await _repositoryManager.Language.GetLanguage(languageId);
                return ex.GenerateBadRequest(_logger, this, $"User update failed", String.Format(_localizer.GetLocalString("updateUserErrorMessage", language.Code), userUpdate.Id));
            }
        }

        [HttpPut("UpdateLanguage")]
        [SwaggerOperation(summary: "Update user language", description: "Update user language and return new token")]
        [SwaggerResponse(200, "Language updated", typeof(JwtAuthResult))]
        [SwaggerResponse(400, "Could not update language", typeof(HttpError))]
        public async Task<IActionResult> UpdateLanguage(int languageId)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int loggedInUserLanguageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            // Update user language and return user if success
            SmartWaste.Model.Core.User? user = await _serviceManager.UserService.UpdateUserLanguage(loggedInUserId, languageId);

            if (user is not null)
                return Ok(_jwtAuthManager.GenerateTokens(user));

            Language language = await _repositoryManager.Language.GetLanguage(loggedInUserLanguageId);
            return BadRequest(new HttpError(_localizer.GetLocalString("couldNotUpdateLanguageErrorMessage", language.Code)));
        }

        [HttpGet("PossibleParents")]
        [SwaggerOperation(summary: "Get all parents", description: $"Return get all possible parents for given type")]
        [SwaggerResponse(200, "List of all possible user parents", typeof(List<BaseUserDto>))]
        [SwaggerResponse(400, "Invalid type", typeof(HttpError))]
        public async Task<IActionResult> GetPossibleParents(int typeId, int? roleId = null)
        {
            _logger.LogInformation($"Get possible parents with type {typeId} and roleId {roleId}");

            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            // Check if type is user and roleId is null
            if (typeId == (int)PossibleParentTypeEnum.User && roleId is null)
                return BadRequest();

            // Convert the integer to the enum
            if (!Enum.IsDefined(typeof(PossibleParentTypeEnum), typeId))
            {
                Language language = await _repositoryManager.Language.GetLanguage(languageId);
                return BadRequest(new HttpError(_localizer.GetLocalString("possibleParentsInvalidTypeErrorMessage", language.Code)));
            }

            // Set enum by type
            var typeEnum = (PossibleParentTypeEnum)typeId;

            return Ok(_mapper.Map<List<BaseUserDto>>(await _repositoryManager.User.GetPossibleParents(loggedInUserId, typeEnum, roleId)));
        }

        [HttpGet("PossibleAssignedUsers")]
        [SwaggerOperation(summary: "Get all possible assigned users", description: $"Get all possible assigned users which can be assigned from logged user")]
        [SwaggerResponse(200, "List of all possible assigned users", typeof(List<BaseUserDto>))]
        public async Task<IActionResult> GetPossibleAssignedUsers(int unitId)
        {
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            _logger.LogInformation($"Get assigned users requsted by {loggedInUserId}");

            var unit = await _repositoryManager.Unit.GetUnit(unitId, false, false);

            if (unit is not null)
            {
                var users = await _repositoryManager.User.GetAllBranchUserChildren(unit.UserId.Value, loggedInUserId, new List<int> { DatabaseConstants.Role.Technician.Id });

                // Return possible assigned users ordered by name
                return Ok(_mapper.Map<List<BaseUserDto>>(users.OrderBy(u => u.Name)));
            }

            return Ok(new List<BaseUserDto>());
        }

        [HttpDelete("{email}")]
        [Authorize(Policy = "AdminDistrubCorporateCustomerPolicy")]
        [SwaggerOperation(summary: "Delete user", description: "Delete user only with flag")]
        [SwaggerResponse(200, "User delete")]
        [SwaggerResponse(400, "Unable to delete user", typeof(HttpError))]
        public async Task<IActionResult> DeleteUser(string email)
        {
            // Get logged in user
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            try
            {
                // Delete user and sub users
                return (await _serviceManager.UserService.DeleteUser(loggedInUserId, email)).GenerateResponse(this);
            }
            catch (Exception ex)
            {
                Language language = await _repositoryManager.Language.GetLanguage(languageId);
                return ex.GenerateBadRequest(_logger, this, $"An error occurred when {loggedInUserId} tried to delete {email}", _localizer.GetLocalString("errorOccurredErrorMessage", language.Code));
            }
        }

        [HttpPut("ChangeUserStatus")]
        [Authorize(Policy = "AdminDistrubCorporateCustomerPolicy")]
        [SwaggerOperation(summary: "Change user status", description: "Set user status to active or inactive")]
        [SwaggerResponse(200, "User status changed")]
        [SwaggerResponse(400, "Can't can't change user status", typeof(HttpError))]
        public async Task<IActionResult> ChangeUserStatus(int userId)
        {
            // Get logged in user claims
            int loggedInUserRole = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.Role).Value);
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            try
            {
                // Change user status
                return (await _serviceManager.UserService.ChangeUserStatus(loggedInUserRole, loggedInUserId, userId, languageId)).GenerateResponse(this);
            }
            catch (Exception ex)
            {
                Language language = await _repositoryManager.Language.GetLanguage(languageId);
                return ex.GenerateBadRequest(_logger, this, $"An error occurred when {loggedInUserId} tried to change status for {userId}", 
                    _localizer.GetLocalString("errorOccurredErrorMessage", language.Code));
            }
        }

        [HttpGet]
        [SwaggerOperation(summary: "Get user data", description: "Get user data for logged in user and if user id is send then data for that user")]
        [SwaggerResponse(200, "Get user", typeof(UserDto))]
        [SwaggerResponse(400, "Can't get this user", typeof(HttpError))]
        public async Task<IActionResult> GetUser(int? userId)
        {
            // Get logged in user claims
            int loggedInUserRole = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.Role).Value);
            int parentId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            // Set user to logged in user if it's null (to get logged in user data)
            int userIdValue = userId ?? parentId;

            // Check logged in user can get user data
            if (((loggedInUserRole == DatabaseConstants.Role.Driver.Id || loggedInUserRole == DatabaseConstants.Role.Technician.Id) && userIdValue != parentId) 
                || (loggedInUserRole != DatabaseConstants.Role.Admin.Id && !await _repositoryManager.User.CanUserChangeData(parentId, userIdValue)
                && userId != null))
            {
                _logger.LogWarning($"User {parentId} tried to get data for user {userId} but they do not have authorization to access this user");
                Language language = await _repositoryManager.Language.GetLanguage(languageId);
                return BadRequest(new HttpError(_localizer.GetLocalString("canNotGetUserErrorMessage", language.Code)));
            }

            // Get user
            UserDto? user = await _serviceManager.UserService.GetUser(userIdValue, languageId);
            return Ok(user);
        }

        [HttpGet("Users")]
        [Authorize(Policy = "AdminDistrubCorporateCustomerPolicy")]
        [SwaggerOperation(summary: "Get users", description: "Get list of all users")]
        [SwaggerResponse(200, "Users list")]
        [SwaggerResponse(400, "Can't get users")]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersInput model)
        {
            // Get logged in user claims
            int loggedInUserRole = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.Role).Value);
            int loggedInUserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int languageId = int.Parse(User.Claims.First(c => c.Type == "LanguageId").Value);

            // Get users with count of users
            PaginatedResponse<UserDto> users = await _serviceManager.UserService.GetUsers(model, loggedInUserRole, languageId, loggedInUserId);
            return Ok(users);
        }

        #region private 

        #endregion
    }
}
