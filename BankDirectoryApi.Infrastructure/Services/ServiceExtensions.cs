using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Infrastructure.Data;
using BankDirectoryApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BankDirectoryApi.Infrastructure.Services
{
    public static class ServiceExtensions
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IATMRepository, ATMRepository>();
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            //// Register Services
            //services.AddScoped<IBankService, BankService>();
            //services.AddScoped<IBranchService, BranchService>();
            //services.AddScoped<IATMService, ATMService>();
            //services.AddScoped<ICardService, CardService>();
        }
      
    }
}
