using AutoMapper;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.ExternalAuthProviders;
using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.TokensHandlers;
using BankDirectoryApi.Application.Interfaces.Related.UserManagement;
using BankDirectoryApi.Common.Helpers;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization
{
    /// <summary>
    /// Service for handling authentication and authorization
    /// </summary>
    public class AuthenticationService:IAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITokenGeneratorService _tokenGenerator;
        private readonly ITokenValidatorService _tokenValidator;
        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService;
        private readonly IRoleService _roleService;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IExternalAuthProviderServiceFactory _externalAuthProviderServiceFactory;

        /// <summary>
        /// Constructor for AuthenticationService
        /// </summary>
        /// <param name="refreshTokenService"></param>
        /// <param name="mapper"></param>
        /// <param name="externalAuthProvider"></param>
        /// <param name="tokenGenerator"></param>
        /// <param name="tokenValidator"></param>
        /// <param name="userService"></param>
        /// <param name="passwordService"></param>
        /// <param name="roleService"></param>
        /// <param name="logger"></param>
        public AuthenticationService(
            IRefreshTokenService refreshTokenService,
            IMapper mapper
            ,ITokenGeneratorService tokenGenerator,
            ITokenValidatorService tokenValidator,
            IUserService userService,
            IPasswordService passwordService,
            IRoleService roleService,
            ILogger<AuthenticationService> logger,
            IExternalAuthProviderServiceFactory externalAuthProviderServiceFactory
            )
             
        {
            _mapper = mapper;
            _refreshTokenService = refreshTokenService;
            _tokenGenerator = tokenGenerator;
            _tokenValidator = tokenValidator;
            _userService = userService;
            _passwordService = passwordService;
            _roleService = roleService;
            _logger = logger;
            _externalAuthProviderServiceFactory = externalAuthProviderServiceFactory;


        }
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="clientInfo"></param>
        /// <returns>The result of the registration process, including authentication tokens.</returns>
        public async Task<Result<AuthDTO>> RegisterAsync(RegisterUserDTO model, ClientInfo clientInfo)
        {
            
            var validationResult = ValidationHelper.ValidateNullModel(model);
            if (validationResult.IsFailed)
                return validationResult.ToResult<AuthDTO>();

            var user = await _userService.CreateUserAsync(model);
           if(user.IsFailed)
                return user.ToResult<AuthDTO>();

            var userClaims = await _userService.GetUserCalimsAsync(user.Value.Id);
            if (userClaims.IsFailed)
                return userClaims.ToResult<AuthDTO>();

            var accessToken =  
                    _tokenGenerator.GenerateAccessToken(user.Value.Id,
                    user.Value.UserName,user.Value.Email,user.Value.Roles, userClaims.Value);
            if (accessToken.IsFailed)
                return accessToken.ToResult<AuthDTO>();

            var refreshTokenResult =  _refreshTokenService.GenerateRefreshTokenEntity(user.Value.Id, clientInfo);
            if (refreshTokenResult.IsFailed)
                return refreshTokenResult.ToResult<AuthDTO>();

            await _refreshTokenService.StoreRefreshTokenAsync(refreshTokenResult.Value.refreshTokenEntity);
              
                return  new AuthDTO
                {
                    AccessToken = accessToken.Value,
                    RefreshToken = refreshTokenResult.Value.refreshToken,
                };
           
        }
        /// <summary>
        /// Logs in a user using an external authentication provider.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="providerName"></param>
        /// <param name="clientInfo"></param>
        /// <returns>The result of the external login process, including authentication tokens.</returns>
        public async Task<Result<AuthDTO>> LoginAsync(LoginUserDTO model, ClientInfo clientInfo)
        {
         
              var validationResult = ValidationHelper.ValidateNullModel(model);
            if (validationResult.IsFailed)
                return validationResult.ToResult<AuthDTO>();

            var checkPasswordResult = await _passwordService.CheckPasswordSignInAsync(model, model.Password, false);

            if (checkPasswordResult.IsFailed)
                return checkPasswordResult.ToResult<AuthDTO>();
            model.UserId = checkPasswordResult.Value;
            var roles = await _roleService.GetRolesAsync(model.UserId);
            var claims = await _userService.GetUserCalimsAsync(model.UserId);

                var accessToken =  _tokenGenerator.GenerateAccessToken
                    (model.UserId,model.UserName,model.Email,roles.Value,claims.Value);
                if (accessToken.IsFailed)
                return accessToken.ToResult<AuthDTO>();
            var refreshTokenResult = 
                    _refreshTokenService.GenerateRefreshTokenEntity(model.UserId, clientInfo);
            if (refreshTokenResult.IsFailed)
                return refreshTokenResult.ToResult<AuthDTO>();

            var storeResult = await _refreshTokenService.StoreRefreshTokenAsync(refreshTokenResult.Value.refreshTokenEntity);
            if (storeResult.IsFailed)
                return storeResult.ToResult<AuthDTO>();
            return new AuthDTO
                {
                    AccessToken = accessToken.Value,
                    RefreshToken = refreshTokenResult.Value.refreshToken,
                };
          
        }
        /// <summary>
        /// Generates a new access token using a refresh token.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="refreshToken"></param>
        /// <param name="clientInfo"></param>
        /// <returns>The result of the token generation process, including the new access token and refresh token.</returns>

        public async Task <Result<AuthDTO>> GenerateAccessTokenFromRefreshTokenAsync(string userId,
            string refreshToken, ClientInfo clientInfo)
        {
           var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId);
            if (validationResult.IsFailed)
                return validationResult.ToResult<AuthDTO>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(refreshToken);
            if (validationResult.IsFailed)
                return validationResult.ToResult<AuthDTO>();


            var user = await _userService.GetUserByIdAsync(userId);
            if (user.IsFailed)
                return user.ToResult<AuthDTO>();

            var refreshTokenEntity =  _refreshTokenService.GenerateRefreshTokenEntity(userId, clientInfo);
            if (refreshTokenEntity.IsFailed)
                return refreshTokenEntity.ToResult<AuthDTO>();

            var claims = await _userService.GetUserCalimsAsync(userId);
                var accessToken =  _tokenGenerator.GenerateAccessToken(
                    userId,user.Value.UserName,user.Value.Email,user.Value.Roles,claims.Value);

            if (accessToken.IsFailed)
                return accessToken.ToResult<AuthDTO>();
           var rotateResult =  await _refreshTokenService.RotateRefreshTokenAsync(
                    refreshToken, refreshTokenEntity.Value.refreshTokenEntity);
            if (rotateResult.IsFailed)
                return rotateResult.ToResult<AuthDTO>();
            return new AuthDTO
                {
                    AccessToken = accessToken.Value,
                    RefreshToken = refreshTokenEntity.Value.refreshToken,
                };
          
        }
        /// <summary>
        /// Logs out a user from the system.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="clientInfo"></param>
        /// <returns>The value of userId.</returns>
        public async Task<Result<string>> LogoutAsync(
            string userId, string sessionId, ClientInfo clientInfo)
        {
           
               var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId);
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(sessionId);
            if (validationResult.IsFailed)
                return validationResult.ToResult<string>();

            var revokeResult =await _refreshTokenService.RevokeAllRefreshTokensAsync(
                    userId, sessionId, clientInfo?.IpAddress);
            if (revokeResult.IsFailed)
                return revokeResult.ToResult<string>();

            return Result.Ok(userId);
         
        }
        /// <summary>
        /// Logs in a user using an external authentication provider.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="providerName"></param>
        /// <param name="clientInfo"></param>
        /// <returns>The result of the external login process, including authentication tokens.</returns>
        public async Task<Result<AuthDTO>> ExternalLoginAsync(string code,string providerName, ClientInfo clientInfo)
        {
          var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(code);
            if (validationResult.IsFailed)
                return validationResult.ToResult<AuthDTO>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(providerName);
            if (validationResult.IsFailed)
                return validationResult.ToResult<AuthDTO>();

            var externalAuthProvider = _externalAuthProviderServiceFactory.GetProvider(providerName);
            var externalLoginInfo = await externalAuthProvider.ManageExternalLogin(code, clientInfo);
            if (externalLoginInfo.IsFailed)
                return externalLoginInfo.ToResult<AuthDTO>();

            UserDTO user;
            var userExists = await _userService.UserExistsByEmailAsync(externalLoginInfo.Value.userDTO.Email);
            if (userExists.IsFailed)
                return userExists.ToResult<AuthDTO>();

            if (!userExists.Value) {
                     var userResult = await _userService.CreateUserAsync(new RegisterUserDTO
                    {
                        Email = externalLoginInfo.Value.userDTO.UserName,
                        UserName = externalLoginInfo.Value.userDTO.Email,
                    });
                if (userResult.IsFailed)
                    return userResult.ToResult<AuthDTO>();
                user = userResult.Value;
            }
            else
            {
                var userResult = await _userService.GetUserByEmailAsync(externalLoginInfo.Value.userDTO.Email);
                if (userResult.IsFailed)
                    return userResult.ToResult<AuthDTO>();
                user = userResult.Value;
            }

                var loginResult = await _userService.AddLoginAsync(user.Id, user.Email, user.UserName,
                    externalLoginInfo.Value.externalAccessToken, providerName);
            if (loginResult.IsFailed)
                return loginResult.ToResult<AuthDTO>();
            var roles = await _roleService.GetRolesAsync(user.Id);
                var claims = await _userService.GetUserCalimsAsync(user.Id);
                var accessToken =  _tokenGenerator.GenerateAccessToken
                    (user.Id, user.UserName, user.Email, roles.Value, claims.Value );
            if(accessToken.IsFailed)
                return accessToken.ToResult<AuthDTO>();
            var refreshTokenResult = 
                    _refreshTokenService.GenerateRefreshTokenEntity(user.Id, clientInfo);
            if (refreshTokenResult.IsFailed)
                return refreshTokenResult.ToResult<AuthDTO>();

            var storeResult =
                await _refreshTokenService.StoreRefreshTokenAsync(refreshTokenResult.Value.refreshTokenEntity);
            if (storeResult.IsFailed)
                return storeResult.ToResult<AuthDTO>();
            return new AuthDTO
                {
                    AccessToken = accessToken.Value,
                    RefreshToken = refreshTokenResult.Value.refreshToken,
                };
        }
        /// <summary>
        /// Validates the provided access token.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>True if the access token is valid; otherwise, false.</returns>
        public async Task<Result<bool>> ValidateAccessTokenAsync(string accessToken)
        {
           var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(accessToken);
            if (validationResult.IsFailed)
                return validationResult.ToResult<bool>().IsFailed;

            var result = await _tokenValidator.ValidateAccessTokenAsync(accessToken);
            if (result.IsFailed)
                return result.ToResult<bool>();
            return Result.Ok(result.Value);
           
        }
        
    }
}
