using BankDirectoryApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankDirectoryApi.Infrastructure.Data
{
    public class DbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(ApplicationDbContext context,
            RoleManager<ApplicationRole> roleManager, 
            UserManager<ApplicationUser> userManager,
            ILogger<DbInitializer> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            // Ensure database is created
            await _context.Database.MigrateAsync();

            // Seed roles
            await SeedRolesAsync();

            // Seed admin user
            await SeedAdminUserAsync();
        }

        private async Task SeedRolesAsync()
        {
            _logger.LogInformation("Seeding roles...");
            // Define roles
            var roles = new[] { "Admin", "User", "Manager" };

            foreach (var roleName in roles)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var role = new ApplicationRole { Name = roleName };
                    await _roleManager.CreateAsync(role);
                    _logger.LogInformation($"Role {roleName} created.");
                }
                else
                {
                    _logger.LogInformation($"Role {roleName} already exists.");
                }
            }
            _logger.LogInformation("Roles seeded successfully.");
        }

        private async Task SeedAdminUserAsync()
        {
            _logger.LogInformation("Seeding admin user...");
            // Check if the admin user exists
            var adminUser = await _userManager.FindByEmailAsync("admin@bankdirectory.com");
            _logger.LogInformation("Admin user already exists.");
            if (adminUser == null)
            {
                // Create the admin user
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@bankdirectory.com",
                };

                var createResult = await _userManager.CreateAsync(adminUser, "abc123");

                if (createResult.Succeeded)
                {
                    _logger.LogInformation("Admin user created successfully.");
                    // Assign roles to the admin user
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation("Admin user assigned to Admin role.");

                }
              

            }
            _logger.LogInformation("Admin user seeded successfully.");
        }
    }
}
