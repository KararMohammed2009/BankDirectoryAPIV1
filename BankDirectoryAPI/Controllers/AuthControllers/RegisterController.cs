using Asp.Versioning;
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.API.Models;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BankDirectoryApi.API.Controllers.AuthControllers
{
  
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Auth/Register")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public RegisterController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
          
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserDTO model)
        {
            var _clientInfo = ClientInfoHelper.GetClientInfo(HttpContext);
            var result = await _authenticationService.RegisterAsync(model, _clientInfo);
            return Ok(new ApiResponse<AuthDTO>(result.Value, null, (int)(HttpStatusCode.OK)));
        }

    }
}
