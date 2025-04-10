using BankDirectoryApi.Application.Interfaces.Related.AuthenticationAndAuthorization.ExternalAuthProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services.Related.AuthenticationAndAuthorization.ExternalAuthProviders
{
    public class ExternalAuthProviderFactory : IExternalAuthProviderServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public ExternalAuthProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IExternalAuthProviderService GetProvider(string providerName)
        {
            if (string.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentNullException(nameof(providerName), "Provider name cannot be null or empty.");
            }


            var providerService = providerName switch
            {
                "Google" => _serviceProvider.GetService(typeof(GoogleAuthProviderService)) as IExternalAuthProviderService,
                _ => null
            };

            if (providerService == null)
            {
                throw new NotImplementedException($"No provider found for {providerName}");
            }

            return providerService;
        }
    }
    }

