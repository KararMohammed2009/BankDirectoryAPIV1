using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs.Auth
{
    public class MicrosoftUserDTO
    {
        public string Mail { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
    }
}
