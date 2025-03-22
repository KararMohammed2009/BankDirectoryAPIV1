using Asp.Versioning;
using BankDirectoryApi.API.Extensions;
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.API.Models;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BankDirectoryApi.API.Controllers.AuthControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Auth/Login")]
    [ApiController]
    public class LoginController: ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public LoginController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO model)
        {
            var _clientInfo = ClientInfoHelper.GetClientInfo(HttpContext);
            var result = await _authenticationService.LoginAsync(model, _clientInfo);
            return Ok(new ApiResponse<AuthDTO>(result,null, (int)HttpStatusCode.OK));
        }
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutUserDTO model)
        {
            var _userId = UserHelper.GetUserId(HttpContext);
            var _clientInfo = ClientInfoHelper.GetClientInfo(HttpContext);
            var result = await _authenticationService.LogoutAsync(_userId,model.SessionId, _clientInfo);
            return Ok(new ApiResponse<bool>(result, null, (int)HttpStatusCode.OK));
        }

    }
}
