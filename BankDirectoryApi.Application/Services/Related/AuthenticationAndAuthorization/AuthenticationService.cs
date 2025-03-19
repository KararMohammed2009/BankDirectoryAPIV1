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
        private readonly IPasswordService _passwordService;
        private readonly IRoleService _roleService;

        public AuthenticationService(
            IRefreshTokenService refreshTokenService,
            IMapper mapper, IExternalAuthProviderService externalAuthProvider
            ,ITokenGeneratorService tokenGenerator,
            ITokenValidatorService tokenValidator,
            IUserService userService,
            IPasswordService passwordService,
            IRoleService roleService
            )
             
        {
            _mapper = mapper;
            _refreshTokenService = refreshTokenService;
            _externalAuthProvider = externalAuthProvider;
            _tokenGenerator = tokenGenerator;
            _tokenValidator = tokenValidator;
            _userService = userService;
            _passwordService = passwordService;
            _roleService = roleService;

        }
   
        public async Task<AuthDTO> RegisterAsync(RegisterUserDTO model, ClientInfo clientInfo)
        {
            try
            {

                if (model == null) throw new Exception("RegisterUserDTO is required");
                var user = await _userService.CreateUserAsync(model);

                var accessToken = await 
                    _tokenGenerator.GenerateAccessTokenAsync(user.Id,user.UserName,user.Email,user.Roles);

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
              
                var result = await _passwordService.CheckPasswordSignInAsync(model, model.Password, false);

                if (!result)
                    throw new Exception("Invalid credentials");
                
                var roles =await  _roleService.GetRolesAsync(model.UserId);
                var accessToken = await _tokenGenerator.GenerateAccessTokenAsync
                    (model.UserId,model.UserName,model.Email,roles);

                var refreshTokenResult = await
                    _refreshTokenService.GenerateRefreshTokenEntityAsync(model.UserId, clientInfo);

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
                if (string.IsNullOrEmpty(userId)) throw new Exception("User id is required");
                if (string.IsNullOrEmpty(refreshToken)) throw new Exception("Refresh token is required");

                var user = await _userService.GetUserByIdAsync(userId);

                var refreshTokenEntity = await _refreshTokenService.GenerateRefreshTokenEntityAsync(user.Id, clientInfo);

                var accessToken = await _tokenGenerator.GenerateAccessTokenAsync(
                    user.Id,user.UserName,user.Email,user.Roles);

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
                    throw new Exception("User id is required");
                }
                if (string.IsNullOrEmpty(sessionId)) throw new Exception("Session id is required");

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
                if (string.IsNullOrEmpty(code)) { throw new Exception("Code is required"); }
                var externalLoginInfo = await _externalAuthProvider.ManageExternalLogin(code, clientInfo);
                var user = await _userService.GetUserByEmailAsync(externalLoginInfo.Email);
                if (user == null) {
                     user = await _userService.CreateUserAsync(new RegisterUserDTO
                    {
                        Email = externalLoginInfo.Email,
                        UserName = externalLoginInfo.Email,
                    });
                }
                await _userService.AddLoginAsync(user.Id,user.Email,user.UserName,externalLoginInfo.)
               

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
                return result;
            }
            catch (Exception ex)
            {
                throw new AuthenticationServiceException("Validate access token failed", ex);
            }
        }
    }
}
