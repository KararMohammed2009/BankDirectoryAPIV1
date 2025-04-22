using Asp.Versioning;
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Interfaces.Related.UserManagement;
using BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Services.Related.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers.AuthControllers
{
    /// <summary>
    /// Controller for managing user-related operations such as retrieving, updating, and managing user accounts.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/User")]
    [ApiController]
    public class UserController :ControllerBase

    {
        private readonly IUserService _userService;
        private readonly IActionGlobalMapper _actionGlobalMapper;
        /// <summary>
        /// Constructor for UserController.
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="actionGlobalMapper"></param>
        public UserController(IUserService userService, IActionGlobalMapper actionGlobalMapper)
        {
            _userService = userService;
            _actionGlobalMapper = actionGlobalMapper;
        }
        /// <summary>
        /// Retrieves the current user's information based on the JWT token.
        /// </summary>
        /// <returns>The current user's information.</returns>
        [Authorize]
        [HttpGet("User")]
        public async Task<IActionResult> GetUser()
        {
            var _userId = UserHelper.GetUserId(HttpContext);
            var result = await _userService.GetUserByIdAsync(_userId);
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }


        /// <summary>
        /// Updates the current user's information based on the provided data.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The updated user's information.</returns>
        [Authorize]
        [HttpPut("User")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO model)
        {
            var result = await _userService.UpdateUserAsync(model);
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }

        /// <summary>
        /// Enables two-factor authentication (2FA) for the current user.
        /// </summary>
        /// <returns>The result of the 2FA enabling process.</returns>
        [Authorize]
        [HttpPost("Enable2fa")]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var _userId = UserHelper.GetUserId(HttpContext);
            var result = await _userService.SetTwoFactorAuthenticationAsync(_userId,true);
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }

        /// <summary>
        /// Disables two-factor authentication (2FA) for the current user.
        /// </summary>
        /// <returns>The result of the 2FA disabling process.</returns>
        [Authorize]
        [HttpPost("Disable2fa")]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var _userId = UserHelper.GetUserId(HttpContext);
            var result = await _userService.SetTwoFactorAuthenticationAsync(_userId, false);
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }

    }
}
