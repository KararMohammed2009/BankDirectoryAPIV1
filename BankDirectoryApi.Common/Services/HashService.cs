using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Services
{
     public class HashService : IHashService
    {
        public string GetHash(string key)
        {
            return BCrypt.Net.BCrypt.HashString(key);
        }
        public bool VerifyHash(string key, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(key, hash);
        }
    }
}
