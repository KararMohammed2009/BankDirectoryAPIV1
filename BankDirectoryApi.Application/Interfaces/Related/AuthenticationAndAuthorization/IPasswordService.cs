using BankDirectoryApi.Application.DTOs.Generic;
using BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization
{
    public interface IPasswordService
    {
        public Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDTO model);
        public Task<Result<AuthDTO>> ResetPasswordAsync(ResetPasswordDTO model, ClientInfo clientInfo);
        public Task<Result<AuthDTO>> ChangePasswordAsync(ChangePasswordDTO model, ClientInfo clientInfo);
    }
}
