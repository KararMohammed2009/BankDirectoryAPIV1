using AutoMapper;
using BankDirectoryApi.Application.DTOs.Generic;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.ExternalAuthProviders;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers;
using BankDirectoryApi.Common.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    public class AuthenticationService:IAuthenticationService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IExternalAuthProvider _externalAuthProvider;
        private readonly IMapper _mapper;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ITokenValidator _tokenValidator;

        public AuthenticationService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IRefreshTokenService refreshTokenService,
            IMapper mapper, IExternalAuthProvider externalAuthProvider
            ,ITokenGenerator tokenGenerator,
            ITokenValidator tokenValidator)
             
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _refreshTokenService = refreshTokenService;
            _externalAuthProvider = externalAuthProvider;
            _tokenGenerator = tokenGenerator;
            _tokenValidator = tokenValidator;

        }
        public async Task<(string? accessToken, string? refreshToken)> GenerateAndStoreTokensAsync(
        IdentityUser user
       , ClientInfo clientInfo)
        {
            var accessToken = await _tokenGenerator.GenerateAccessTokenAsync(user);

            var refreshTokenResult = await _refreshTokenService.GenerateRefreshTokenEntityAsync(user.Id, clientInfo);
            if (refreshTokenResult.refreshTokenEntity == null || 
                string.IsNullOrEmpty(refreshTokenResult.refreshToken)) return (null, null);
            await _refreshTokenService.StoreRefreshTokenAsync(refreshTokenResult.refreshTokenEntity);

            return (accessToken, refreshTokenResult.refreshToken);
        }
        
        public async Task<Result<AuthDTO>> RegisterAsync(RegisterUserDTO model, ClientInfo clientInfo)
        {
            var user = _mapper.Map<RegisterUserDTO, IdentityUser>(model);
            if (user == null) throw new ValidationException("Cannot map RegisterUserDTO to IdentityUser");
            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded) 
                    throw new ValidationException("Cannot create user");
                var tokens = await GenerateAndStoreTokensAsync(user, clientInfo);
                return Result<AuthDTO>.SuccessResult(new AuthDTO
                {
                    AccessToken = tokens.accessToken,
                    RefreshToken = tokens.refreshToken,
                });
            }
            catch (Exception ex)
            {
                throw new ValidationException("Cannot create user", ex);
            }
        }

      public async Task<Result<AuthDTO>> LoginAsync(LoginUserDTO model, ClientInfo clientInfo)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            throw new ValidationException("Cannot find email");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                throw new AuthenticationException("Cannot sign in user");
            var tokens = await GenerateAndStoreTokensAsync(user, clientInfo);
            return Result<AuthDTO>.SuccessResult(new AuthDTO
            {
                AccessToken = tokens.accessToken,
                RefreshToken = tokens.refreshToken,
            });
        }
        public async Task<Result<AuthDTO>> GenerateAccessTokenFromRefreshTokenAsync(string userId,
            string refreshToken, ClientInfo clientInfo)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new NotFoundException("User not found");
            var refreshTokenEntity = await _refreshTokenService.GenerateRefreshTokenEntityAsync(user.Id, clientInfo);
            if (refreshTokenEntity.refreshTokenEntity == null) throw
                    new TokenHandlingException("Failed to generate refresh token");
            var accessToken = await _tokenGenerator.GenerateAccessTokenAsync(user);
            if (string.IsNullOrEmpty(accessToken)) throw new TokenHandlingException("Failed to generate access token");
            var rotateSucceeded = await _refreshTokenService.RotateRefreshTokenAsync(
                refreshToken, refreshTokenEntity.refreshTokenEntity);
            if(!rotateSucceeded) throw new TokenHandlingException("Failed to rotate refresh token");
            return Result<AuthDTO>.SuccessResult(new AuthDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenEntity.refreshToken,
            });
        }
        public async Task<Result<bool>> LogoutAsync(
            string userId, string sessionId, ClientInfo clientInfo)
        {
            var succeed = await _refreshTokenService.RevokeAllRefreshTokensAsync(
                userId, sessionId, clientInfo?.IpAddress);
            if (!succeed)
            {
               throw new TokenHandlingException("Cannot revoke refresh tokens");
            }
            return Result<bool>.SuccessResult(true);
        }
        public async Task<Result<AuthDTO>> ExternalLoginAsync(string code, ClientInfo clientInfo)
        {
            var externalLoginInfo = await _externalAuthProvider.ManageExternalLogin(code, clientInfo);
            if (!externalLoginInfo.Success)
            {
                throw new AuthenticationException("Cannot manage external login");
            }
            if (externalLoginInfo.Response == null)
            {
                throw new AuthenticationException("Cannot manage external login");
            }
            return Result<AuthDTO>.SuccessResult(externalLoginInfo.Response);
        }
        public async Task<Result<bool>> ValidateAccessToken(string accessToken)
        {
            var result = await _tokenValidator.ValidateAccessTokenAsync(accessToken);
            return Result<bool>.SuccessResult(result);
        }
    }
}
