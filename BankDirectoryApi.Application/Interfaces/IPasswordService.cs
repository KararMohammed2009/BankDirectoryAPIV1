using BankDirectoryApi.Application.DTOs;
using BankDirectoryApi.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces
{
    public interface IPasswordService
    {
        public Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDTO model);
        public Task<Result<AuthDTO>> ResetPasswordAsync(ResetPasswordDTO model, ClientInfo clientInfo);
        public Task<Result<AuthDTO>> ChangePasswordAsync(ChangePasswordDTO model, ClientInfo clientInfo);
    }
}
