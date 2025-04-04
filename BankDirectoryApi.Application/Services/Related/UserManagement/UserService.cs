using AutoMapper;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.UserManagement;
using BankDirectoryApi.Common.Extensions;
using BankDirectoryApi.Infrastructure;
using BankDirectoryApi.Infrastructure.Identity;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Configuration.Provider;
using System.Net;
using System.Security.Claims;

namespace BankDirectoryApi.Application.Services.Related.UserManagement
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger _logger;

        public UserService(
            IMapper mapper, UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager
            , ILogger<UserService> logger
            )
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        public async Task<Result<List<UserDTO>>> GetAllUsersAsync(UserFilterDTO model, CancellationToken cancellationToken)
        {

            if (model == null)
                return Result.Fail(new Error("model is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var users = await IdentityExceptionHelper.Execute(() => _userManager.Users.ToListAsync(cancellationToken), _logger);
            
            return Result.Ok( _mapper.Map<List<UserDTO>>(users));

        }
        public async Task<Result<UserDTO>> GetUserByIdAsync(string userId)
        {

            if (string.IsNullOrEmpty(userId))
                return Result.Fail(new Error("UserId is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Fail(new Error($"Get User by Id({userId}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));

            var roles = await IdentityExceptionHelper.Execute(() => _userManager.GetRolesAsync(user), _logger);
            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Roles = roles;
            return Result.Ok(userDTO);
        }
        public async Task<Result<UserDTO>> GetUserByEmailAsync(string email)
        {

            if (string.IsNullOrEmpty(email))
                return Result.Fail(new Error("Email is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var user = await IdentityExceptionHelper.Execute(() => _userManager.FindByEmailAsync(email), _logger);
            if (user == null)
                return Result.Fail(new Error($"Get User by Email({email}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            return Result.Ok(_mapper.Map<UserDTO>(user));

        }
        public async Task<Result<UserDTO>> GetUserByUserNameAsync(string userName)
        {
            
                if (string.IsNullOrEmpty(userName)) 
                return Result.Fail(new Error("UserName is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            var user = await IdentityExceptionHelper.Execute(() => _userManager.FindByNameAsync(userName),_logger);
                if (user == null)
                   return Result.Fail(new Error($"Get User by UserName({userName}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            return Result.Ok(_mapper.Map<UserDTO>(user));
           
        }
        public async Task<Result<UserDTO>> UserExistsByEmailAsync(string email)
        {
           
                if (string.IsNullOrEmpty(email)) 
                 return Result.Fail(new Error("Email is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            var user = await IdentityExceptionHelper.Execute(() => _userManager.FindByEmailAsync(email),_logger);
            if (user == null)
                return Result.Fail(new Error($"Get User by Email({email}) failed by UserManager<ApplicationUser>")
                .WithMetadata("StatusCode", HttpStatusCode.NotFound));
           
                return Result.Ok( _mapper.Map<UserDTO>(user));
            
        }


        public async Task<Result<UserDTO>> UpdateUserAsync(UpdateUserDTO model)
        {
            
                if (model == null) 
                return Result.Fail(new Error("Model is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
                 
            var user = _mapper.Map<ApplicationUser>(model);
                var updateResult = await IdentityExceptionHelper.Execute(() => _userManager.UpdateAsync(user),_logger);
                if (!updateResult.Succeeded)
                {
                   return Result.Fail(new Error($"Update User failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(updateResult);

            }

                var existedRoles = await IdentityExceptionHelper.Execute(() =>
                _userManager.GetRolesAsync(user),_logger);
            if (existedRoles == null)
            {
                return Result.Fail(new Error($"Get User Roles failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }
                var desiredRoles = model.RoleNames ?? new List<string>();

                // Remove roles
                var rolesToRemove = existedRoles.Except(desiredRoles).ToList();
                if (rolesToRemove.Any())
                {
                    var removeResult = await IdentityExceptionHelper.Execute(() =>
                    _userManager.RemoveFromRolesAsync(user, rolesToRemove),_logger);
                    if (!removeResult.Succeeded)
                    {
                        return Result.Fail(new Error($"Update User failed by UserManager<ApplicationUser>")
                            .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(removeResult);
                    }
                }

                // Add roles
                var rolesToAdd = desiredRoles.Except(existedRoles).ToList();
                if (rolesToAdd.Any())
                {
                    var addResult = await IdentityExceptionHelper.Execute(() => 
                    _userManager.AddToRolesAsync(user, rolesToAdd),_logger);
                    if (!addResult.Succeeded)
                    {
                return Result.Fail(new Error($"Update User failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(addResult);
                }
                }

                return Result.Ok(_mapper.Map<UserDTO>(user));
          
        }
        public async Task<Result<string>> DeleteUserAsync(string userId)
        {
            
                if (string.IsNullOrEmpty(userId)) 
                return Result.Fail(new Error("UserId is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var user = await IdentityExceptionHelper.Execute(() => _userManager.FindByIdAsync(userId),_logger);

                if (user == null)
                return Result.Fail(new Error($"Get User by Id({userId}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));

            var result = await IdentityExceptionHelper.Execute(() => _userManager.DeleteAsync(user),_logger);
           
                if (!result.Succeeded)
                return Result.Fail(new Error($"Delete User failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(result);
            return Result.Ok(userId);
           
        }
        public async Task<Result<UserDTO>> CreateUserAsync(RegisterUserDTO model)
        {
            if (model == null)
                return Result.Fail(new Error("model is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var user = await IdentityExceptionHelper.Execute(() =>
                _userManager.FindByEmailAsync(model.Email), _logger);
            if (user != null)
            {
                return Result.Fail(new Error($"User with email({model.Email}) already exists")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }

            user = _mapper.Map<ApplicationUser>(model);

            var result = await IdentityExceptionHelper.Execute(() =>
                _userManager.CreateAsync(user, model.Password), _logger);
            if (!result.Succeeded)
            {
                return Result.Fail(new Error($"Create User failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(result);
            }

            result = await IdentityExceptionHelper.Execute(() => 
            _userManager.AddToRolesAsync(user, model.Roles),_logger);
            if (!result.Succeeded)
            {

                return Result.Fail(
                   new Error($"Add User to Roles failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(result);
               
            }

            return Result.Ok(_mapper.Map<UserDTO>(user));
        }
        public async Task<Result<string>> ConfirmEmailAsync(string email, string token)
        {
            
                if (string.IsNullOrEmpty(email)) 
                return Result.Fail(new Error("Email is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            if (string.IsNullOrEmpty(token))
                return Result.Fail(new Error("Token is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var identityUser = await IdentityExceptionHelper.Execute(() =>
            _userManager.FindByEmailAsync(email),_logger);

            if (identityUser == null)
            {
                return Result.Fail(new Error($"Get User by Email({email}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            }
            var result = await IdentityExceptionHelper.Execute(() => 
            _userManager.ConfirmEmailAsync(identityUser, token), _logger);
            if (!result.Succeeded)
            { return Result.Fail(
                new Error("Email confirmation failed by UserManager<ApplicationUser>")
                .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(result); 
            }

                return Result.Ok(email);
          
        }

        public async Task<Result<bool>> IsEmailConfirmedAsync(UserDTO model)
        {
           
                if (model == null) 
                return Result.Fail(new Error("Model is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            var user = _mapper.Map<ApplicationUser>(model);
                return Result.Ok( await
                     IdentityExceptionHelper.Execute(() =>
                    _userManager.IsEmailConfirmedAsync(user),_logger));
          
        }
        public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(string email)
        {
           
                if (string.IsNullOrEmpty(email)) 
                return Result.Fail(new Error("Email is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var user = await IdentityExceptionHelper.Execute(() => 
            _userManager.FindByEmailAsync(email),_logger);
            if (user == null)
            {
                return Result.Fail(new Error($"Get User by Email({email}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            }

                var result = await IdentityExceptionHelper.Execute(() => 
                _userManager.GenerateEmailConfirmationTokenAsync(user),_logger);
                if (string.IsNullOrEmpty(result))
            {
                return Result.Fail(new Error($"Generate Email Confirmation Token failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            }
            
                return Result.Ok(result);
           
        }

        public async Task<Result<string>> AddLoginAsync(string id, string email, string name, string externalAccessToken, string providerName)
        {
            
                if (string.IsNullOrEmpty(id)) 
                return Result.Fail(new Error("id is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            if (string.IsNullOrEmpty(email))
                return Result.Fail(new Error("email is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            if (string.IsNullOrEmpty(name))
                return Result.Fail(new Error("name is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            if (string.IsNullOrEmpty(externalAccessToken))
                return Result.Fail(new Error("externalAccessToken is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            if (string.IsNullOrEmpty(providerName))
                return Result.Fail(new Error("providerName is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var user = await IdentityExceptionHelper.Execute(() => 
            _userManager.FindByEmailAsync(email),_logger);

                if (user == null)
                return Result.Fail(new Error($"Get User by Email({email}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, id),
                        new Claim(ClaimTypes.Email, email),
                        new Claim(ClaimTypes.Name, name),
                    };

                // Create ClaimsIdentity
                var identity = new ClaimsIdentity(claims, providerName);

                // Create ClaimsPrincipal
                var claimsPrincipal = new ClaimsPrincipal(identity);
                var info = new ExternalLoginInfo(
                                  claimsPrincipal,
                                  providerName,
                                  externalAccessToken,
                                  email
                              );
                var result = await IdentityExceptionHelper.Execute(() => 
                _userManager.AddLoginAsync(user,info),_logger);

                if (!result.Succeeded)
            {
                return Result.Fail(new Error($"Add Login failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(result);
            }
                return Result.Ok(email);
        }
        public async Task<Result<string>> SetTwoFactorAuthenticationAsync(string userId, bool enabled)
        {
           
                if (string.IsNullOrEmpty(userId))
              return Result.Fail(new Error("userId is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));
            var user = await IdentityExceptionHelper.Execute(() => 
            _userManager.FindByIdAsync(userId),_logger);

                if (user == null)
                return Result.Fail(new Error($"Get User by Id({userId}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));

            var result =await IdentityExceptionHelper.Execute(() => 
            _userManager.SetTwoFactorEnabledAsync(user, enabled),_logger);
                if (!result.Succeeded)
                {
                    return Result.Fail(new Error($"Set Two Factor Authentication failed by UserManager<ApplicationUser>")
                        .WithMetadata("StatusCode", HttpStatusCode.BadRequest)).IncludeIdentityErrors(result);
            }
                return Result.Ok(userId);
           
        }
        public async Task<Result<Dictionary<string, string>>> GetUserCalimsAsync(string userId)
        {
           
                if (string.IsNullOrEmpty(userId)) 
                return Result.Fail(new Error("userId is required")
                    .WithMetadata("StatusCode", HttpStatusCode.BadRequest));

            var user = await IdentityExceptionHelper.Execute(() =>
            _userManager.FindByIdAsync (userId),_logger);

                if(user ==null) 
                return Result.Fail(new Error($"Get User by Id({userId}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("StatusCode", HttpStatusCode.NotFound));

            var claims = await IdentityExceptionHelper.Execute(() => 
            _userManager.GetClaimsAsync(user),_logger);
                Dictionary<string,string> result = new Dictionary<string,string>();
                if(claims !=null && claims.Any())
                {
                    foreach(var claim in claims)
                    {
                        result.Add(claim.Type,claim.Value);
                    }
                }
                return Result.Ok(result);

           
        }
    }
}


