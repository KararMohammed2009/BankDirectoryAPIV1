using BankDirectoryApi.Application.Interfaces.Auth;
using BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.ExternalAuthProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BankDirectoryApi.API.Controllers.AuthControllers.ExternalAuthProviders
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleController : ControllerBase
    {
        private readonly GoogleAuthProvider _googleAuthProvider;
        public GoogleController(GoogleAuthProvider googleAuthProvider)
        {
            _googleAuthProvider = googleAuthProvider;
        }
        [HttpGet("ExternalLogin")]
        public async Task<IActionResult> ExternalLogin(string code)
        {
            
            var response = await _googleAuthProvider.ManageExternalLogin(code);
            if (!response.Success)
            {
                return BadRequest(response.errors);
            }
            return Ok(response.Response);
        }
    }
}
