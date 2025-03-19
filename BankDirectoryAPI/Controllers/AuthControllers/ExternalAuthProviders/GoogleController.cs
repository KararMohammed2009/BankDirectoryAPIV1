
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.ExternalAuthProviders;
using BankDirectoryApi.Application.Services.Related.UserManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BankDirectoryApi.API.Controllers.AuthControllers.ExternalAuthProviders
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly string _providerName = "Google";
        public GoogleController(AuthenticationService authenticationService)
        {
           _authenticationService = authenticationService;
        }
        [HttpGet("ExternalLogin")]
        public async Task<IActionResult> ExternalLogin(string code)
        {
            var clientInfos = ClientInfoHelper.GetClientInfo(HttpContext);
            var response = await _authenticationService.ExternalLoginAsync(code, _providerName, clientInfos);
            return Ok(response);
        }
    }
}
