
using BankDirectoryApi.API.Helpers;
using BankDirectoryApi.API.Mappings.Interfaces;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using Microsoft.AspNetCore.Mvc;

namespace BankDirectoryApi.API.Controllers.AuthControllers.ExternalAuthProviders
{
    /// <summary>
    /// Controller for handling Google authentication.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly string _providerName = "Google";
        private readonly IActionGlobalMapper _actionGlobalMapper;
        /// <summary>
        /// Constructor for GoogleController.
        /// </summary>
        /// <param name="authenticationService"></param>
        /// <param name="actionGlobalMapper"></param>
        public GoogleController(IAuthenticationService authenticationService , IActionGlobalMapper actionGlobalMapper)
        {
            _actionGlobalMapper = actionGlobalMapper;
           _authenticationService = authenticationService;
        }
        /// <summary>
        /// Initiates the Google authentication process.
        /// </summary>
        /// <param name="code"></param>
        /// <returns>The result of the authentication process .</returns>
        [HttpGet("ExternalLogin")]
        public async Task<IActionResult> ExternalLogin(string code)
        {
            var clientInfos = ClientInfoHelper.GetClientInfo(HttpContext);
            var response = await _authenticationService.ExternalLoginAsync(code, _providerName, clientInfos);
            return _actionGlobalMapper.MapResultToApiResponse(response);
        }
    }
}
