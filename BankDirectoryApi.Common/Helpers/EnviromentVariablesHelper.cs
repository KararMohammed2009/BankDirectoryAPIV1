
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BankDirectoryApi.Common.Helpers
{
    public class EnviromentVariablesHelper 
    {

        public static string GetByKey(IConfiguration configuration,string key)
        {
            return Environment.GetEnvironmentVariable(key)??
                throw new InvalidOperationException($"{key} is not configured!");
        }
    }
}
