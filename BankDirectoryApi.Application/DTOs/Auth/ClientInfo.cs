using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.DTOs.Auth
{
    public class ClientInfo
    {
        public string IpAddress { get; set; } // Client's IP address
        public string UserAgent { get; set; } // Client's user agent

    }
}
