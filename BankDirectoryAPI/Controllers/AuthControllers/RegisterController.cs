using Asp.Versioning;
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers.AuthControllers
{
    /// <summary>
    /// Controller for handling user registration operations.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Auth/Register")]
    [ApiController]
    [AllowAnonymous]
    public class RegisterController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IActionGlobalMapper _actionGlobalMapper;
        /// <summary>
        /// Constructor for RegisterController.
        /// </summary>
        /// <param name="authenticationService"></param>
        /// <param name="actionGlobalMapper"></param>
        public RegisterController(IAuthenticationService authenticationService,
            IActionGlobalMapper actionGlobalMapper)
        {
            _authenticationService = authenticationService;
            _actionGlobalMapper = actionGlobalMapper;

        }
        /// <summary>
        /// Handles user registration.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The result of the registration process , including authentication tokens.</returns>
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserDTO model)
        {
            var _clientInfo = ClientInfoHelper.GetClientInfo(HttpContext);
            var result = await _authenticationService.RegisterAsync(model, _clientInfo);
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }
        /// <summary>
        /// Handles user registration by an admin.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The result of the registration process , including user details.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("RegisterByAdmin")]
        public async Task<IActionResult> RegisterByAdmin(RegisterUserByAdminDTO model)
        {
            var result = await _authenticationService.RegisterByAdminAsync(model);
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }

    }
}
