using BankDirectoryApi.Application.DTOs;
using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services
{
    public class UserService : IUserService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IIdentityService _identityService;
        public UserService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IIdentityService identityService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _identityService = identityService;
        }
        public async Task<ExternalLoginResponseDTO> ExternalLogin(ExternalLoginRequestDTO request)
        {
            throw new NotImplementedException();
        }
    }
}
