using Asp.Versioning;
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.ThirdParties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers.AuthControllers
{
    /// <summary>
    /// Controller for handling user login and logout operations.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Auth/Login")]
    [ApiController]
    public class LoginController: ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IActionGlobalMapper _actionGlobalMapper;
        /// <summary>
        /// Constructor for LoginController.
        /// </summary>
        /// <param name="authenticationService"></param>
        /// <param name="actionGlobalMapper"></param>
        public LoginController(IAuthenticationService authenticationService
            ,IActionGlobalMapper actionGlobalMapper)
        {
            _authenticationService = authenticationService;
            _actionGlobalMapper = actionGlobalMapper;
        }
        /// <summary>
        /// Handles user login.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The result of the login process.</returns>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO model)
        {
            var _clientInfo = ClientInfoHelper.GetClientInfo(HttpContext);
                var result = await _authenticationService.LoginAsync(model, _clientInfo);
                return _actionGlobalMapper.MapResultToApiResponse(result);

        }
        /// <summary>
        /// Handles user logout.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The result of the logout process.</returns>
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutUserDTO model)
        {
            var _userId = UserHelper.GetUserId(HttpContext);
            var _clientInfo = ClientInfoHelper.GetClientInfo(HttpContext);
            var result = await _authenticationService.LogoutAsync(_userId,model.SessionId, _clientInfo);
            return _actionGlobalMapper.MapResultToApiResponse(result);
            
        }

    }
}
