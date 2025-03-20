using Asp.Versioning;
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers.AuthControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Login")]
    [ApiController]
    public class LoginController: ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly ClientInfo _clientInfo;
        private readonly string _userId;
        public LoginController(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _clientInfo = ClientInfoHelper.GetClientInfo(HttpContext);
            _userId = UserHelper.GetUserId(HttpContext);
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login(LoginUserDTO model)
        {
            var result = await  _authenticationService.LoginAsync(model, _clientInfo);
            return Ok(result);
        }
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout(string sessionId)
        {
            var result = await _authenticationService.LogoutAsync(_userId,sessionId, _clientInfo);
            return Ok(result);
        }

    }
}
