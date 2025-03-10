using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs.Auth
{

    public class UpdateUserDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IEnumerable<string> RolesNames { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactorEnabled { get; set; }


    }

}
