using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Services
{
    public interface IHashService
    {
        string GetHash(string key);
        bool VerifyHash(string key, string hash);
    }
}
