using BankDirectoryApi.Application.DTOs.Auth;
using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces
{
    //public interface IUserService
    //{
    //    Task<AuthenticationDTO> RegisterAsync(RegisterRequest request);
    //    Task<AuthenticationDTO> LoginAsync(LoginRequest request);
    //    Task<AuthenticationDTO?> ExternalLoginAsync(string provider, string idToken);
    //}
    public interface IUserService
{
    Task<User> GetUserByIdAsync(string userId);
    Task<User> GetUserByUsernameAsync(string username);
    Task<IdentityResult> CreateUserAsync(User user, string password);
    Task<JwtResponse> GenerateTokensAsync(User user);
    Task<JwtResponse> RefreshTokenAsync(string refreshToken);
    Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword);
    Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
    Task<IdentityResult> LogoutAsync(string refreshToken);
    Task<ExternalLoginResponse> ExternalLoginAsync(string provider, string accessToken);
}

}
