using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BankDirectoryApi.Infrastructure.Identity
{
    public class IdentityDbContextFactory : IDesignTimeDbContextFactory<MyIdentityDbContext>
    {
        public MyIdentityDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<MyIdentityDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new MyIdentityDbContext(optionsBuilder.Options);
        }
    }
}
