using Asp.Versioning;
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.ThirdParties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers.AuthControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Auth/Login")]
    [ApiController]
    public class LoginController: ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IActionGlobalMapper _actionGlobalMapper;
        private readonly ISmsService _smsService;
        private readonly IEmailService _emailService;

        public LoginController(IAuthenticationService authenticationService
            ,IActionGlobalMapper actionGlobalMapper,
            ISmsService smsService,
            IEmailService emailService)
        {
            _authenticationService = authenticationService;
            _actionGlobalMapper = actionGlobalMapper;
            _smsService = smsService;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO model)
        {
            var _clientInfo = ClientInfoHelper.GetClientInfo(HttpContext);
                var result = await _authenticationService.LoginAsync(model, _clientInfo);
                return _actionGlobalMapper.MapResultToApiResponse(result);

        }
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutUserDTO model)
        {
            var _userId = UserHelper.GetUserId(HttpContext);
            var _clientInfo = ClientInfoHelper.GetClientInfo(HttpContext);
            var result = await _authenticationService.LogoutAsync(_userId,model.SessionId, _clientInfo);
            return _actionGlobalMapper.MapResultToApiResponse(result);
            
        }

    }
}
