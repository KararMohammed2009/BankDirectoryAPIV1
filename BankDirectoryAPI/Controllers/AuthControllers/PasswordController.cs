using Asp.Versioning;
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers.AuthControllers
{
    /// <summary>
    /// Controller for handling password-related operations such as changing, resetting, and requesting password resets.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Password")]
    [ApiController]
    public class PasswordController : ControllerBase

    {
        private readonly IPasswordService _passwordService;
        private readonly IActionGlobalMapper _actionGlobalMapper;
        /// <summary>
        /// Constructor for PasswordController.
        /// </summary>
        /// <param name="passwordService"></param>
        /// <param name="actionGlobalMapper"></param>
        public PasswordController(IPasswordService passwordService,IActionGlobalMapper actionGlobalMapper)
        {
            _passwordService = passwordService;
            _actionGlobalMapper = actionGlobalMapper;
        }
        /// <summary>
        /// Handles changing the user's password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The result of the password change process.</returns>
        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            var _clientInfo = ClientInfoHelper.GetClientInfo(HttpContext);
            var _userId = UserHelper.GetUserId(HttpContext);
            var result = await _passwordService.ChangePasswordAsync(
                _userId,model.CurrentPassword, model.NewPassword, _clientInfo);
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }

        /// <summary>
        /// Handles the process of requesting a password reset.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The result of the password reset request.</returns>
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            var result = await _passwordService.ForgotPasswordAsync(model.Email);
            return _actionGlobalMapper.MapResultToApiResponse(result.ToResult());
        }

        /// <summary>
        /// Handles the process of resetting the user's password using a reset code.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The result of the password reset process.</returns>
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            var result = await _passwordService.ResetPasswordAsync(model.Email,model.Code,model.NewPassword);
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }

    }
}
