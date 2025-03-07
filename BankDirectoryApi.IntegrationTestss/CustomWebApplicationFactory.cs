using BankDirectoryApi.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using BankDirectoryApi.IntegrationTestss.Data;
namespace BankDirectoryApi.IntegrationTestss
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program> 
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's DbContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ApplicationDbContext>)); // Replace ApplicationDbContext with your context.

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add a new DbContext using an in-memory database.
                services.AddDbContext<ApplicationDbContext>(options => // Replace ApplicationDbContext with your context.
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a scoped DbContext instance
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>(); // Replace ApplicationDbContext with your context.

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    // You can seed the database here with test data if needed.
                     SeedData.PopulateTestData(db);
                }
                //Configure app configuration for testing.
                builder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile("appsettings.Test.json", optional: true, reloadOnChange: true); //Add test specific settings.
                });
            });
        }
    }

}
