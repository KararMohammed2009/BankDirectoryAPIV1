//using Azure.Core;
//using BankDirectoryApi.Application.DTOs;
//using BankDirectoryApi.Application.DTOs.Auth;
//using BankDirectoryApi.Application.Interfaces;
//using BankDirectoryApi.Application.Interfaces.Auth;
//using BankDirectoryApi.Application.Services.ExternalAuthProviders;
//using BankDirectoryApi.Infrastructure.Identity;
//using Microsoft.AspNetCore.Identity;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BankDirectoryApi.Application.Services
//{
//    public class UserService : IUserService
//    {
//        private readonly SignInManager<IdentityUser> _signInManager;
//        private readonly UserManager<IdentityUser> _userManager;
//        private readonly IIdentityService _identityService;
//        private readonly IExternalAuthProvider _externalAuthProvider;
//        public UserService(SignInManager<IdentityUser> signInManager, 
//            UserManager<IdentityUser> userManager, IIdentityService identityService,IExternalAuthProvider externalAuthProvider)
//        {
//            _signInManager = signInManager;
//            _userManager = userManager;
//            _identityService = identityService;
//            _externalAuthProvider = externalAuthProvider;
//        }
        
//    }
//}
