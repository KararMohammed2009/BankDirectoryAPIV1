using Asp.Versioning;
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers.AuthControllers
{
  
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Register")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly ClientInfo _clientInfo;
        public RegisterController(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _clientInfo = ClientInfoHelper.GetClientInfo(HttpContext);
        }

        [HttpGet("Register")]
        public async Task<IActionResult> Register(RegisterUserDTO model)
        {
            var result = await _authenticationService.RegisterAsync(model, _clientInfo);
            return Ok(result);
        }

    }
}
