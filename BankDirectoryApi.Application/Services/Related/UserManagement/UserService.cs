using AutoMapper;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Interfaces.Related.UserManagement;
using BankDirectoryApi.Common.Errors;
using BankDirectoryApi.Common.Extensions;
using BankDirectoryApi.Common.Helpers;
using BankDirectoryApi.Common.Services;
using BankDirectoryApi.Domain.Classes.Pagination;
using BankDirectoryApi.Domain.Classes.Specifications;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.Entities.Identity;
using BankDirectoryApi.Infrastructure;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BankDirectoryApi.Application.Services.Related.UserManagement
{
    /// <summary>
    /// Service class for user management operations.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<UserService> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        /// <summary>
        /// Constructor for UserService.
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="logger"></param>
        public UserService(
            IMapper mapper, UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager
            , ILogger<UserService> logger,
            IDateTimeProvider dateTimeProvider
            )
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }
        /// <summary>
        /// Retrieves all users based on the provided filter criteria.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The list of users that match the filter criteria.</returns>
        public async Task<Result<List<UserDTO>>> GetAllUsersAsync(UserFilterDTO model, CancellationToken cancellationToken)
        {
            var validationResult = ValidationHelper.ValidateNullModel(model, "model");
            if (validationResult.IsFailed) return validationResult.ToResult<List<UserDTO>>();

            

            var users = await IdentityExceptionHelper.Execute(async () =>
            {
                var spec = new Specification<ApplicationUser>()
                {
                    Criteria = ExpressionFilterHelper.CreateFilter<ApplicationUser>(model),
                    IsPagingEnabled = model.PaginationInfo != null,
                    PageNumber = model.PaginationInfo?.PageNumber,
                    PageSize = model.PaginationInfo?.PageSize,
                    AsNoTracking = true,
                };
                var response = new PaginatedResponse<ApplicationUser>();
                var query = _userManager.Users.AsQueryable();

                if (model != null)
                {
                    query = query.Where(spec.Criteria);
                }

                if (spec.IsPagingEnabled && spec.PageNumber.HasValue && spec.PageSize.HasValue)
                {
                    query = query.Skip((spec.PageNumber.Value - 1) * spec.PageSize.Value)
                        .Take(spec.PageSize.Value);
                    response.Pagination = new PaginationInfo
                    {
                        PageNumber = spec.PageNumber.Value,
                        PageSize = spec.PageSize.Value
                    };
                }

                if (spec.AsNoTracking)
                {
                    query = query.AsNoTracking();
                }
                response.TotalItems = await query.CountAsync(cancellationToken);
                response.Items = await query.ToListAsync(cancellationToken);
                return response;
            }, _logger);
            var roles = await IdentityExceptionHelper.Execute(() => _roleManager.Roles.ToListAsync(cancellationToken), _logger);
            var usersDtos = _mapper.Map<List<UserDTO>>(users.Items);
            foreach (var item in usersDtos)
            {
                var user = users.Items.Find(o => o.Id == item.Id);
                var userRoles = await IdentityExceptionHelper.Execute(() => _userManager.GetRolesAsync(user!), _logger);
                item.RolesNames = userRoles;
            }
            return Result.Ok(usersDtos);
        }
        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The user with the specified ID.</returns>
        public async Task<Result<UserDTO>> GetUserByIdAsync(string userId)
        {
            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed) return validationResult.ToResult<UserDTO>();


            var user = await IdentityExceptionHelper.Execute(()=>
            _userManager.FindByIdAsync(userId),_logger);
            if (user == null)
                return Result.Fail(new Error($"Get User by Id({userId}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));

            var roles = await IdentityExceptionHelper.Execute(() => _userManager.GetRolesAsync(user), _logger);
            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.RolesNames = roles;
            return Result.Ok(userDTO);
        }
        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The user with the specified email address.</returns>
        public async Task<Result<UserDTO>> GetUserByEmailAsync(string email)
        {

            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(email, "email");
            if (validationResult.IsFailed) return validationResult.ToResult<UserDTO>();

            var user = await IdentityExceptionHelper.Execute(() => _userManager.FindByEmailAsync(email), _logger);
            if (user == null)
                return Result.Fail(new Error($"Get User by Email({email}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            var roles = await IdentityExceptionHelper.Execute(() => _userManager.GetRolesAsync(user), _logger);
            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.RolesNames = roles;
            return Result.Ok(userDTO);

        }
        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>The user with the specified username.</returns>
        public async Task<Result<UserDTO>> GetUserByUserNameAsync(string userName)
        {

            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userName, "userName");
            if (validationResult.IsFailed) return validationResult.ToResult<UserDTO>();

            var user = await IdentityExceptionHelper.Execute(() => _userManager.FindByNameAsync(userName),_logger);
                if (user == null)
                   return Result.Fail(new Error($"Get User by UserName({userName}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            var roles = await IdentityExceptionHelper.Execute(() => _userManager.GetRolesAsync(user), _logger);
            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.RolesNames = roles;
            return Result.Ok(userDTO);
           
        }
        /// <summary>
        /// Checks if a user exists by their email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>True if the user exists, false otherwise.</returns>
        public async Task<Result<bool>> UserExistsByEmailAsync(string email)
        {

            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(email, "email");
            if (validationResult.IsFailed) return validationResult.ToResult<bool>();

            var user = await IdentityExceptionHelper.Execute(() => _userManager.FindByEmailAsync(email),_logger);
            
                return Result.Ok(user !=null);
        }
        /// <summary>
        /// Updates a user with the provided information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The updated user.</returns>
        public async Task<Result<UserDTO>> UpdateUserAsync(UpdateUserDTO model)
        {

            var validationResult = ValidationHelper.ValidateNullModel(model, "model");
            if (validationResult.IsFailed) return validationResult.ToResult<UserDTO>();

            var oldUser = await IdentityExceptionHelper.Execute(() =>
            _userManager.FindByIdAsync(model.Id), _logger);
            if (oldUser == null)
                return Result.Fail(new Error($"Get User by Id({model.Id}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));

            var updatedUser = _mapper.Map(model,oldUser);
                var updateResult = await IdentityExceptionHelper.Execute(() => _userManager.UpdateAsync(updatedUser),_logger);
                if (!updateResult.Succeeded)
                {
                   return Result.Fail(new Error($"Update User failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.OperationFailed))
                    .IncludeIdentityErrors(updateResult);

            }

                var existedRoles = await IdentityExceptionHelper.Execute(() =>
                _userManager.GetRolesAsync(updatedUser),_logger);
            if (existedRoles == null)
            {
                return Result.Fail(new Error($"Get User Roles failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.OperationFailed));
            }
                var desiredRoles = model.RolesNames ?? new List<string>();

                // Remove roles
                var rolesToRemove = existedRoles.Except(desiredRoles).ToList();
                if (rolesToRemove.Any())
                {
                    var removeResult = await IdentityExceptionHelper.Execute(() =>
                    _userManager.RemoveFromRolesAsync(updatedUser, rolesToRemove),_logger);
                    if (!removeResult.Succeeded)
                    {
                        return Result.Fail(new Error($"Update User failed by UserManager<ApplicationUser>")
                            .WithMetadata("ErrorCode", CommonErrors.OperationFailed)).IncludeIdentityErrors(removeResult);
                    }
                }

                // Add roles
                var rolesToAdd = desiredRoles.Except(existedRoles).ToList();
                if (rolesToAdd.Any())
                {
                    var addResult = await IdentityExceptionHelper.Execute(() => 
                    _userManager.AddToRolesAsync(updatedUser, rolesToAdd),_logger);
                    if (!addResult.Succeeded)
                    {
                return Result.Fail(new Error($"Update User failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.OperationFailed)).IncludeIdentityErrors(addResult);
                }
                }

                return Result.Ok(_mapper.Map<UserDTO>(updatedUser));
          
        }
        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The value of user id.</returns>
        public async Task<Result<string>> DeleteUserAsync(string userId)
        {

            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();

            var user = await IdentityExceptionHelper.Execute(() => _userManager.FindByIdAsync(userId),_logger);

                if (user == null)
                return Result.Fail(new Error($"Get User by Id({userId}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));

            var result = await IdentityExceptionHelper.Execute(() => _userManager.DeleteAsync(user),_logger);
           
                if (!result.Succeeded)
                return Result.Fail(new Error($"Delete User failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.OperationFailed)).IncludeIdentityErrors(result);
            return Result.Ok(userId);
           
        }
        /// <summary>
        /// Creates a new user with the provided information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The created user.</returns>
        public async Task<Result<UserDTO>> CreateUserAsync(RegisterUserDTO model)
        {
            var validationResult = ValidationHelper.ValidateNullModel(model, "model");
            if (validationResult.IsFailed) return validationResult.ToResult<UserDTO>();

            var user = await IdentityExceptionHelper.Execute(() =>
                _userManager.FindByEmailAsync(model.Email), _logger);
            if (user != null)
            {
                return Result.Fail(new Error($"User with email({model.Email}) already exists")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceAlreadyExists));
            }

            user = _mapper.Map<ApplicationUser>(model);
            user.CreationDate = _dateTimeProvider.UtcNow.Value;
            var result = await IdentityExceptionHelper.Execute(() =>
                _userManager.CreateAsync(user, model.Password), _logger);
            if (!result.Succeeded)
            {
                return Result.Fail(new Error($"Create User failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.OperationFailed)).IncludeIdentityErrors(result);
            }

           
            return Result.Ok(_mapper.Map<UserDTO>(user));
        }
        /// <summary>
        /// Confirms a user's email address using the provided token.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns>The value of the user id.</returns>
        public async Task<Result<string>> ConfirmEmailAsync(string email, string token)
        {

            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(email, "email");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();

            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(token, "token");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();

            var identityUser = await IdentityExceptionHelper.Execute(() =>
            _userManager.FindByEmailAsync(email),_logger);

            if (identityUser == null)
            {
                return Result.Fail(new Error($"Get User by Email({email}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            }
            var result = await IdentityExceptionHelper.Execute(() => 
            _userManager.ConfirmEmailAsync(identityUser, token), _logger);
            if (!result.Succeeded)
            { return Result.Fail(
                new Error("Email confirmation failed by UserManager<ApplicationUser>")
                .WithMetadata("ErrorCode", CommonErrors.OperationFailed)).IncludeIdentityErrors(result); 
            }

                return Result.Ok(email);
          
        }
        /// <summary>
        /// Checks if a user's email is confirmed.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>True if the email is confirmed, false otherwise.</returns>
        public async Task<Result<bool>> IsEmailConfirmedAsync(UserDTO model)
        {

            var validationResult = ValidationHelper.ValidateNullModel(model, "model");
            if (validationResult.IsFailed) return validationResult.ToResult<bool>();
            var user = await IdentityExceptionHelper.Execute(() =>
            _userManager.FindByEmailAsync(model.Email), _logger);
            if (user == null)
            {
                return Result.Fail(new Error($"Get User by Email({model.Email}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            }
            return Result.Ok( await
                     IdentityExceptionHelper.Execute(() =>
                    _userManager.IsEmailConfirmedAsync(user),_logger));
          
        }
        /// <summary>
        /// Generates a token for email confirmation.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The value of email confirmation token.</returns>
        public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(string email)
        {

            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(email, "email");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();

            var user = await IdentityExceptionHelper.Execute(() => 
            _userManager.FindByEmailAsync(email),_logger);
            if (user == null)
            {
                return Result.Fail(new Error($"Get User by Email({email}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
            }

                var result = await IdentityExceptionHelper.Execute(() => 
                _userManager.GenerateEmailConfirmationTokenAsync(user),_logger);
                if (string.IsNullOrEmpty(result))
            {
                return Result.Fail(new Error($"Generate Email Confirmation Token failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.OperationFailed));
            }
            
                return Result.Ok(result);
           
        }
        /// <summary>
        /// External login for a user using the provided information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <param name="externalAccessToken"></param>
        /// <param name="providerName"></param>
        /// <returns>The value of the user id.</returns>
        public async Task<Result<string>> AddLoginAsync(string id, string email, string name, string externalAccessToken, string providerName)
        {

            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(id, "id");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(email, "email");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(name, "name");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullModel(externalAccessToken, "externalAccessToken");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();
            validationResult = ValidationHelper.ValidateNullModel(providerName, "providerName");


            var user = await IdentityExceptionHelper.Execute(() => 
            _userManager.FindByEmailAsync(email),_logger);

                if (user == null)
                return Result.Fail(new Error($"Get User by Email({email}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));
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
                    .WithMetadata("ErrorCode", CommonErrors.OperationFailed)).IncludeIdentityErrors(result);
            }
                return Result.Ok(email);
        }
        /// <summary>
        /// Set Two-Factor Authentication enabled or disabled for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enabled"></param>
        /// <returns>The value of the user id.</returns>
        public async Task<Result<string>> SetTwoFactorAuthenticationAsync(string userId, bool enabled)
        {

            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed) return validationResult.ToResult<string>();

            var user = await IdentityExceptionHelper.Execute(() => 
            _userManager.FindByIdAsync(userId),_logger);

                if (user == null)
                return Result.Fail(new Error($"Get User by Id({userId}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));

            var result =await IdentityExceptionHelper.Execute(() => 
            _userManager.SetTwoFactorEnabledAsync(user, enabled),_logger);
                if (!result.Succeeded)
                {
                    return Result.Fail(new Error($"Set Two Factor Authentication failed by UserManager<ApplicationUser>")
                        .WithMetadata("ErrorCode", CommonErrors.OperationFailed)).IncludeIdentityErrors(result);
            }
                return Result.Ok(userId);
           
        }
        /// <summary>
        /// Get the claims of a user by their ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The claims of the user .</returns>
        public async Task<Result<Dictionary<string, string>>> GetUserCalimsAsync(string userId)
        {

            var validationResult = ValidationHelper.ValidateNullOrWhiteSpaceString(userId, "userId");
            if (validationResult.IsFailed) return validationResult.ToResult<Dictionary<string,string>>();

            var user = await IdentityExceptionHelper.Execute(() =>
            _userManager.FindByIdAsync (userId),_logger);

                if(user ==null) 
                return Result.Fail(new Error($"Get User by Id({userId}) failed by UserManager<ApplicationUser>")
                    .WithMetadata("ErrorCode", CommonErrors.ResourceNotFound));

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


