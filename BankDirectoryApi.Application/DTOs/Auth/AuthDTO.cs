using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs.Auth
{
    public class AuthDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }

    }

}
