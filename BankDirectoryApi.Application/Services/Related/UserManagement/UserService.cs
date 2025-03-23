using AutoMapper;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Application.Exceptions;
using BankDirectoryApi.Application.Interfaces.Related.UserManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration.Provider;
using System.Security.Claims;

namespace BankDirectoryApi.Application.Services.Related.UserManagement
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            IMapper mapper, UserManager<IdentityUser> userManager
            , SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            try
            {
                var identityusers = await _userManager.Users.ToListAsync();
                return _mapper.Map<IEnumerable<UserDTO>>(identityusers);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("Get all Users failed by UserManager<IdentityUser>", ex);
            }
        }
        public async Task<UserDTO> GetUserByIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) throw new Exception("UserId is required");
                var identityUser = await _userManager.FindByIdAsync(userId);
                if (identityUser == null) throw new Exception($"Get User by Id ({userId}) failed by UserManager<IdentityUser>");
                var roles = await _userManager.GetRolesAsync(identityUser);
                var userDTO = _mapper.Map<UserDTO>(identityUser);
                userDTO.Roles = roles;
                return userDTO;
            }
            catch (Exception ex)
            {
                throw new UserServiceException($"Get User by Id failed", ex);
            }
        }
        public async Task<UserDTO> GetUserByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) throw new Exception("Email is required");
                var identityUser = await _userManager.FindByEmailAsync(email);
                if (identityUser == null)
                    throw new Exception($"Get User by Email({email}) failed by UserManager<IdentityUser>");
                return _mapper.Map<UserDTO>(identityUser);
            }
            catch (Exception ex)
            {
                throw new UserServiceException($"Get User by Email failed", ex);
            }
        }
        public async Task<UserDTO> GetUserByUserNameAsync(string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName)) throw new Exception("userName is required");
                var identityUser = await _userManager.FindByNameAsync(userName);
                if (identityUser == null)
                    throw new Exception($"Get User by username({userName}) failed by UserManager<IdentityUser>");
                return _mapper.Map<UserDTO>(identityUser);
            }
            catch (Exception ex)
            {
                throw new UserServiceException($"Get User by Email failed", ex);
            }
        }
        public async Task<UserDTO?> UserExistsByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) throw new Exception("Email is required");
                var identityUser = await _userManager.FindByEmailAsync(email);
                if (identityUser == null)
                  return null;
                return _mapper.Map<UserDTO>(identityUser);
            }
            catch (Exception ex)
            {
                throw new UserServiceException($"Get User by Email failed", ex);
            }
        }


        public async Task<UserDTO> UpdateUserAsync(UpdateUserDTO user)
        {
            try
            {
                if (user == null) throw new Exception("User is required");
                var identityUser = _mapper.Map<IdentityUser>(user);
                var updateResult = await _userManager.UpdateAsync(identityUser);
                if (!updateResult.Succeeded)
                {
                    throw new Exception($"Failed to update user(id = {user.UserId}) by UserManager<IdentityUser>");
                }

                var existedRoles = await _userManager.GetRolesAsync(identityUser);
                var desiredRoles = user.RoleNames ?? new List<string>();

                // Remove roles
                var rolesToRemove = existedRoles.Except(desiredRoles).ToList();
                if (rolesToRemove.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(identityUser, rolesToRemove);
                    if (!removeResult.Succeeded)
                    {
                        throw new Exception($"Failed to remove roles from user(id = {identityUser.Id}) by UserManager<IdentityUser>");
                    }
                }

                // Add roles
                var rolesToAdd = desiredRoles.Except(existedRoles).ToList();
                if (rolesToAdd.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(identityUser, rolesToAdd);
                    if (!addResult.Succeeded)
                    {
                        throw new Exception($"Failed to add roles to user(id = {identityUser.Id}) by UserManager<IdentityUser>");
                    }
                }

                return _mapper.Map<UserDTO>(identityUser);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("Update User failed", ex);
            }
        }
        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) throw new Exception("UserId is required");
                var identityUser = await _userManager.FindByIdAsync(userId);
                if (identityUser == null) throw new Exception($"Cannot find user by id({userId}) by UserManager<IdentityUser>");
                var result = await _userManager.DeleteAsync(identityUser);
                if (!result.Succeeded) throw new Exception($"Failed to delete user(id = {userId}) by UserManager<IdentityUser>");
                return true;
            }
            catch (Exception ex)
            {
                throw new UserServiceException("Delete User failed", ex);
            }
        }
        public async Task<UserDTO> CreateUserAsync(RegisterUserDTO user)
        {
            try
            {
              
                if (user == null) throw new Exception("Model is required");
               
                var identityUser = await _userManager.FindByEmailAsync(user.Email);
                if (identityUser != null)
                {
                    return _mapper.Map<UserDTO>(identityUser);
                }

                identityUser = _mapper.Map<IdentityUser>(user);

                var result = await _userManager.CreateAsync(identityUser, user.Password);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user by UserManager<IdentityUser>");
                }

                result = await _userManager.AddToRolesAsync(identityUser, user.Roles);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to assign user to given roles by UserManager<IdentityUser>");
                }


                return _mapper.Map<UserDTO>(identityUser);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("Create User failed", ex);
            }
        }
        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) throw new Exception("Email is required");
                if (string.IsNullOrEmpty(token)) throw new Exception("Token is required");
                var identityUser = await _userManager.FindByEmailAsync(email);
                if (identityUser == null) throw new Exception($"Cannot find user by email({email}) by UserManager<IdentityUser>");
                var result = await _userManager.ConfirmEmailAsync(identityUser, token);
                if (!result.Succeeded) throw new Exception("Email confirmation failed by UserManager<IdentityUser>");
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                throw new UserServiceException("Confirm Email failed", ex);
            }
        }

        public async Task<bool> IsEmailConfirmedAsync(UserDTO user)
        {
            try
            {
                if (user == null) throw new Exception("User is required");
                var identityUser = _mapper.Map<IdentityUser>(user);
                return await _userManager.IsEmailConfirmedAsync(identityUser);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("Is Email Confirmed failed", ex);
            }
        }
        public async Task<string> GenerateEmailConfirmationTokenAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) throw new Exception("Email is required");
                var identityUser = await _userManager.FindByEmailAsync(email);
                if (identityUser == null) throw new Exception($"Cannot find user by email({email}) by UserManager<IdentityUser>");
                var result = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                if (result == null) throw new Exception("Email confirmation token generation failed by UserManager<IdentityUser>");
                return result;
            }
            catch (Exception ex)
            {
                throw new UserServiceException("Generate Email Confirmation Token failed", ex);
            }
        }

        public async Task<bool> AddLoginAsync(string id, string email, string name, string externalAccessToken, string providerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(id)) { throw new Exception("id is null"); }
                if (!string.IsNullOrEmpty(email)) { throw new Exception("email is null"); }
                if (!string.IsNullOrEmpty(name)) { throw new Exception("name is null"); }
                if (!string.IsNullOrEmpty(externalAccessToken)) { throw new Exception("externalAccessToken is null"); }
                if (!string.IsNullOrEmpty(providerName)) { throw new Exception("providerName is null"); }

                var identityUser = await _userManager.FindByEmailAsync(email);
                if (identityUser == null) throw new Exception("Email is required");
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
                var result = await _userManager.AddLoginAsync(identityUser,info);
                if (!result.Succeeded) throw new Exception("Add Login failed by UserManager<IdentityUser>");
                return result.Succeeded;


            }catch(Exception ex)
            {
                throw new UserServiceException("Add Login Failed", ex);
            }
        }
        public async Task<bool> SetTwoFactorAuthenticationAsync(string userId, bool enabled)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("userId is required");
                }
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new Exception($"Cannot find user by id({userId}) by UserManager<IdentityUser>");
                var result =await  _userManager.SetTwoFactorEnabledAsync(user, enabled);
                if (!result.Succeeded)
                {
                    throw new Exception($"Set Two Factor Enabled failed by UserManager<IdentityUser>");
                }
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                throw new UserServiceException("Set Two Factor Authentication failed", ex);
            }
        }
        public async Task<Dictionary<string, string>> GetUserCalimsAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) {
                    throw new Exception("userId is required");
                }
                var identityUser = await _userManager.FindByIdAsync (userId);
                if(identityUser ==null) throw new Exception($"Cannot find user by id({userId}) by UserManager<IdentityUser>");
                var claims = await _userManager.GetClaimsAsync(identityUser);
                Dictionary<string,string> result = new Dictionary<string,string>();
                if(claims !=null && claims.Any())
                {
                    foreach(var claim in claims)
                    {
                        result.Add(claim.Type,claim.Value);
                    }
                }
                return result;

            }
            catch (Exception ex)
            {
                throw new UserServiceException("Get User Calims failed", ex);
            }
        }
    }
}


