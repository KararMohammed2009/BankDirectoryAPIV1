using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using BankDirectoryApi.Domain.Entities;
using Azure.Core;
using BankDirectoryApi.Application.DTOs.Auth;
using BankDirectoryApi.Application.Services.ExternalAuthProviders;
using BankDirectoryApi.Application.Interfaces.Auth;
using BankDirectoryApi.Infrastructure.Identity;
namespace BankDirectoryApi.Application.Services
{
    public class AuthService
    {

    }
}