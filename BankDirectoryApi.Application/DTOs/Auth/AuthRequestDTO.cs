using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs.Auth
{
    public class AuthRequestDTO
    {
        public string idToken { get; set; }
        public string Provider { get; set; }    
    }
}
