using Asp.Versioning;
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Interfaces.Related.UserManagement;
using BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Services.Related.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers.AuthControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/User")]
    [ApiController]
    public class UserController :ControllerBase

    {
        private readonly IUserService _userService;
        private readonly ClientInfo _clientInfo;
        private readonly string _userId;
        public UserController(IUserService userService)
        {
            _userService = userService;
            _clientInfo = ClientInfoHelper.GetClientInfo(HttpContext);
            _userId = UserHelper.GetUserId(HttpContext);
        }
        // Get user User
        [Authorize]
        [HttpGet("User")]
        public async Task<IActionResult> GetUser()
        {
            var result = await _userService.GetUserByIdAsync(_userId);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers(int page = 1, int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Invalid page or pageSize.");
            }
            var result = await _userService.GetUserByIdAsync(_userId);
            return Ok(result);
        }

        // Update user User
        [Authorize]
        [HttpPut("User")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO model)
        {
            var result = await _userService.UpdateUserAsync(model);
            return Ok(result);
        }
        // Enable 2FA
        [Authorize]
        [HttpPost("enable-2fa")]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var result = await _userService.SetTwoFactorAuthenticationAsync(_userId,true);
            return Ok(result);
        }

        // Disable 2FA
        [Authorize]
        [HttpPost("disable-2fa")]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var result = await _userService.SetTwoFactorAuthenticationAsync(_userId, false);
            return Ok(result);
        }

    }
}
