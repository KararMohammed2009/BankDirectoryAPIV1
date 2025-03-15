using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs.Related.AuthenticationAndAuthorization
{
    public class GoogleUserResponseDTO
    {
        public string Sub { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

    }
}
