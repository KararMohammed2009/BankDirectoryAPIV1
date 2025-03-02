using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using BankDirectoryApi.Domain.Entities;
using Azure.Core;
using BankDirectoryApi.Application.DTOs.Auth;
using BankDirectoryApi.Application.Services.ExternalAuthProviders;
using BankDirectoryApi.Application.Interfaces.Auth;
using BankDirectoryApi.Infrastructure.Identity;
namespace BankDirectoryApi.Application.Services
{
    public class AuthService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IEnumerable<IExternalAuthProvider> _externalAuthProviders;
        public AuthService(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager, 
            IIdentityService identityService,
            IEnumerable<IExternalAuthProvider> externalAuthProviders)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _identityService = identityService;
            _externalAuthProviders = externalAuthProviders;
        }
        public async Task<AuthResponseDTO> ExternalLoginAsync(AuthRequestDTO request)
        {

            var provider = _externalAuthProviders.FirstOrDefault(p => p.ProviderName.Equals(request.Provider, StringComparison.OrdinalIgnoreCase));
            if (provider == null) {
                return new AuthResponseDTO
                {
                    Success = false,
                    Errors = new[] { new IdentityError { Description = "Invalid or Unsupported external provider" } }
                }
            ;
                var externalUserInfo = await provider.ValidateAndGetUserAsync(request.idToken);
            
          

            var user = await _userManager.FindByLoginAsync(provider, externalUserInfo.ProviderKey);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = externalUserInfo.Response.,
                    Email = externalUserInfo.Email,
                    EmailConfirmed = true
                };

                var createUserResult = await _userManager.CreateAsync(user);
                if (!createUserResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to create external user.");
                }

                await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, externalUserInfo.ProviderKey, provider));
            }

            // Generate JWT Token
            var token = _jwtTokenGenerator.GenerateJwtToken(user);

            return new AuthResponseDTO { Token = token, UserId = user.Id };
        }

    }
}