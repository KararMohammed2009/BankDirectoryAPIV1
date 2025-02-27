using AutoMapper;
using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankDirectoryApi.Infrastructure.Identity;
using Microsoft.SqlServer.Server;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using BankDirectoryApi.Application.DTOs.Auth;
namespace YourProject.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEnumerable<IExternalAuthProvider> _externalAuthProviders; //inject here.

        public UserService(UserManager<User> userManager, IIdentityService identityService, 
            IMapper mapper, SignInManager<User> signInManager, IConfiguration configuration
            , IEnumerable<IExternalAuthProvider> externalAuthProviders)
        {
            _userManager = userManager;
            _identityService = identityService;
            _mapper = mapper;
            _signInManager = signInManager;
            _configuration = configuration;
            _externalAuthProviders = externalAuthProviders;
        }

        public async Task<AuthenticationDTO> RegisterAsync(RegisterRequest request)
        {
            var user = _mapper.Map<User>(request);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                var token = await _identityService.GenerateJwtToken(user);
                return new AuthenticationDTO { Token = token, Success = true };
            }
            return new AuthenticationDTO { Success = false, Errors = result.Errors };
        }

        public async Task<AuthenticationDTO> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                var token = await _identityService.GenerateJwtToken(user);
                return new AuthenticationDTO { Token = token, Success = true };
            }
            return new AuthenticationDTO { Success = false, Errors = new[] { new IdentityError { Description = "Invalid credentials" } } };
        }

        public async Task<AuthenticationDTO?> ExternalLoginAsync(string provider, string idToken)
        {
            var authProvider = _externalAuthProviders.FirstOrDefault(p => p.GetType().Name.ToLower().Contains(provider.ToLower()));

            if (authProvider == null)
            {
                return new AuthenticationDTO { Success = false, Errors = new[] { new IdentityError { Description = "Invalid provider." } } };
            }

            var (success, user, response) = await authProvider.ValidateAndGetUserAsync(idToken);

            if (!success || user == null)
            {
                return response;
            }

            var token = await _identityService.GenerateJwtToken(user);
            return new AuthenticationDTO { Token = token, Success = true };
        }

    }
}
