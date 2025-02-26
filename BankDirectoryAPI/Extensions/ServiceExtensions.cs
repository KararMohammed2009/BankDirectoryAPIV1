

using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Application.Services;
using BankDirectoryApi.Infrastructure.Data;
using BankDirectoryApi.Infrastructure.Repositories;
using BankDirectoryApi.Application.Services;
using Microsoft.EntityFrameworkCore;
using BankDirectoryApi.Application.Mappings;

namespace BankDirectoryApi.API.Extensions
{
    public static class ServiceExtensions
    {
        
        public static void AddApplicationMappers(this IServiceCollection services)
        {

            // Register Automapper for dtos
            services.AddAutoMapper(typeof(BankProfile));
            services.AddAutoMapper(typeof(BranchProfile));
            services.AddAutoMapper(typeof(BankWithBranchesProfile));

        }

        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Register application-layer services
            services.AddScoped<IBankService, BankService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IATMService, ATMService>();
            services.AddScoped<ICardService, CardService>();
        }


        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register database context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories, for example:
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IATMRepository, ATMRepository>();
            services.AddScoped<ICardRepository, CardRepository>();
        }
    }
}
