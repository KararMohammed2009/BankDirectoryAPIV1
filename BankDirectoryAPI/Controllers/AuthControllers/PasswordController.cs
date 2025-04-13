using Asp.Versioning;
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers.AuthControllers
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Password")]
    [ApiController]
    public class PasswordController : ControllerBase

    {
        private readonly IPasswordService _passwordService;
        private readonly string _userId;
        private readonly IHttpContextAccessor _contextAccessor;
        public PasswordController(IPasswordService passwordService, IHttpContextAccessor contextAccessor)
        {
            _passwordService = passwordService;
            _contextAccessor = contextAccessor;
            _userId = UserHelper.GetUserId(_contextAccessor.HttpContext);
        }
        // Change password
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            var _clientInfo = ClientInfoHelper.GetClientInfo(HttpContext);
            var result = await _passwordService.ChangePasswordAsync(
                _userId,model.CurrentPassword, model.NewPassword, _clientInfo);
            return Ok(result);
        }

        // Request password reset
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            var result = await _passwordService.ForgotPasswordAsync(model.Email);
            return Ok(result);
        }

        // Reset password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            var result = await _passwordService.ResetPasswordAsync(_userId,model.Token,model.NewPassword);
            return Ok(result);
        }

    }
}
