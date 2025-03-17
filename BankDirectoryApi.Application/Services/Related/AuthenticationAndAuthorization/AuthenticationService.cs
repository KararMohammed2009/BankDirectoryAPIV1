using AutoMapper;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.ExternalAuthProviders;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers;
using BankDirectoryApi.Application.Interfaces.Related.UserManagement;
using BankDirectoryApi.Common.Exceptions;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    public class AuthenticationService:IAuthenticationService
    {
        private readonly IExternalAuthProviderService _externalAuthProvider;
        private readonly IMapper _mapper;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITokenGeneratorService _tokenGenerator;
        private readonly ITokenValidatorService _tokenValidator;
        private readonly IUserService _userService;

        public AuthenticationService(
            IRefreshTokenService refreshTokenService,
            IMapper mapper, IExternalAuthProviderService externalAuthProvider
            ,ITokenGeneratorService tokenGenerator,
            ITokenValidatorService tokenValidator,
            IUserService userService)
             
        {
            _mapper = mapper;
            _refreshTokenService = refreshTokenService;
            _externalAuthProvider = externalAuthProvider;
            _tokenGenerator = tokenGenerator;
            _tokenValidator = tokenValidator;
            _userService = userService;

        }
   
        public async Task<AuthDTO> RegisterAsync(RegisterUserDTO model, ClientInfo clientInfo)
        {
            try
            {

                if (model == null) throw new ValidationException("RegisterUserDTO is required");
                var user = await _userService.CreateUserAsync(model);
                if (user == null) 
                    throw new Exception("Cannot create user by UserService");
                var accessToken = await _tokenGenerator.GenerateAccessTokenAsync(user);

                var refreshTokenResult = await _refreshTokenService.GenerateRefreshTokenEntityAsync(user.Id, clientInfo);

               await _refreshTokenService.StoreRefreshTokenAsync(refreshTokenResult.refreshTokenEntity);
              
                return  new AuthDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshTokenResult.refreshToken,
                };
            }
            catch (Exception ex)
            {
                throw new AuthenticationServiceException("Register user failed", ex);
            }
        }

        public async Task<AuthDTO> LoginAsync(LoginUserDTO model, ClientInfo clientInfo)
        {
            try
            {
                if (model == null) throw new ValidationException("LoginUserDTO is required");
                if (string.IsNullOrEmpty(model.Email)) throw new ValidationException("Email is required");
                if (string.IsNullOrEmpty(model.Password)) throw new ValidationException("Password is required");

                var user = await _userService.GetUserByEmailAsync(model.Email);
                if (user == null)
                    throw new ValidationException("Cannot find email by UserService");

                var result = await _userService.CheckPasswordSignInAsync(user, model.Password, false);

                if (!result)
                    throw new AuthenticationException("Invalid credentials");

                var accessToken = await _tokenGenerator.GenerateAccessTokenAsync(user);

                var refreshTokenResult = await _refreshTokenService.GenerateRefreshTokenEntityAsync(user.Id, clientInfo);

                await _refreshTokenService.StoreRefreshTokenAsync(refreshTokenResult.refreshTokenEntity);
                return new AuthDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshTokenResult.refreshToken,
                };
            }
            catch (Exception ex)
            {
                throw new AuthenticationServiceException("Login failed", ex);
            }
        }
        public async Task<AuthDTO> GenerateAccessTokenFromRefreshTokenAsync(string userId,
            string refreshToken, ClientInfo clientInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) throw new ValidationException("User id is required");
                if (string.IsNullOrEmpty(refreshToken)) throw new ValidationException("Refresh token is required");

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null) throw new ValidationException("Cannot find user by UserService");

                var refreshTokenEntity = await _refreshTokenService.GenerateRefreshTokenEntityAsync(user.Id, clientInfo);

                var accessToken = await _tokenGenerator.GenerateAccessTokenAsync(user);

                await _refreshTokenService.RotateRefreshTokenAsync(
                    refreshToken, refreshTokenEntity.refreshTokenEntity);

                return new AuthDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshTokenEntity.refreshToken,
                };
            }
            catch (Exception ex)
            {
                throw new AuthenticationServiceException("Generate access token from refresh token failed", ex);
            }
        }
        public async Task<bool> LogoutAsync(
            string userId, string sessionId, ClientInfo clientInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new ValidationException("User id is required");
                }
                if (string.IsNullOrEmpty(sessionId)) throw new ValidationException("Session id is required");

                await _refreshTokenService.RevokeAllRefreshTokensAsync(
                    userId, sessionId, clientInfo?.IpAddress);

                return true;
            }
            catch (Exception ex)
            {
                throw new AuthenticationServiceException("Logout failed", ex);
            }
        }
        public async Task<AuthDTO> ExternalLoginAsync(string code, ClientInfo clientInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(code)) { throw new ValidationException("Code is required"); }
                var externalLoginInfo = await _externalAuthProvider.ManageExternalLogin(code, clientInfo);
                
                return externalLoginInfo.Response;
            }
            catch (Exception ex)
            {
                throw new AuthenticationServiceException("External login failed", ex);
            }
        }
        public async Task<bool> ValidateAccessToken(string accessToken)
        {
            try
            {
                var result = await _tokenValidator.ValidateAccessTokenAsync(accessToken);
                if (result == false) throw new ValidationException("Invalid access token");
                return result;
            }
            catch (Exception ex)
            {
                throw new AuthenticationServiceException("Validate access token failed", ex);
            }
        }
    }
}
