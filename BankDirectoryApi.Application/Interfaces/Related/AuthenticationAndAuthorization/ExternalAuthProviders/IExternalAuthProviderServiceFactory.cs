using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.ExternalAuthProviders
{
    /// <summary>
    /// Factory interface for creating instances of external authentication provider services.
    /// </summary>
    public interface IExternalAuthProviderServiceFactory
    {
        /// <summary>
        /// Creates an instance of the specified external authentication provider service.
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns>The instance of the external authentication provider service.</returns>
        IExternalAuthProviderService GetProvider(string providerName);
    }
}
