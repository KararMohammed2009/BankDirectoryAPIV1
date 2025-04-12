using Asp.Versioning;
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.API.Mappings.Classes;
using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.API.Models;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BankDirectoryApi.API.Controllers.AuthControllers
{
  
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Auth/Register")]
    [ApiController]
    [AllowAnonymous]
    public class RegisterController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IActionGlobalMapper _actionGlobalMapper;

        public RegisterController(IAuthenticationService authenticationService,
            IActionGlobalMapper actionGlobalMapper)
        {
            _authenticationService = authenticationService;
            _actionGlobalMapper = actionGlobalMapper;

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserDTO model)
        {
            var _clientInfo = ClientInfoHelper.GetClientInfo(HttpContext);
            var result = await _authenticationService.RegisterAsync(model, _clientInfo);
            return _actionGlobalMapper.MapResultToApiResponse(result);
        }

    }
}
