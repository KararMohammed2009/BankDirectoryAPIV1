using BankDirectoryApi.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourProject.Infrastructure.Identity;

namespace BankDirectoryApi.Application.Interfaces
{
    public interface IUserService
    {
        public Task<ExternalLoginResponseDTO> ExternalLogin(ExternalLoginRequestDTO request);
    }
}
