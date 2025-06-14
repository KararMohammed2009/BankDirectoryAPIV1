﻿using BankDirectoryApi.Common.Services;
using BankDirectoryApi.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BankDirectoryApi.Infrastructure.Data
{
    public class DbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DbInitializer> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        public DbInitializer(ApplicationDbContext context,
            RoleManager<ApplicationRole> roleManager, 
            UserManager<ApplicationUser> userManager,
            IDateTimeProvider dateTimeProvider,
            ILogger<DbInitializer> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task InitializeAsync()
        {
           
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
            var adminUser = await _userManager.FindByEmailAsync("admin@bankdirectoryapi.com");
            
            if (adminUser == null)
            {
                // Create the admin user
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@bankdirectoryapi.com",
                    CreationDate =_dateTimeProvider.UtcNow.Value,
                };

                var createResult = await _userManager.CreateAsync(adminUser, "Abc123$$");

                if (createResult.Succeeded)
                {
                    _logger.LogInformation("Admin user created successfully.");
                    // Assign roles to the admin user
                    createResult = await _userManager.AddToRoleAsync(adminUser, "Admin");
                    if (createResult.Succeeded)
                    {
                        _logger.LogInformation("Admin user assigned to Admin role.");
                    }
                    else
                    {
                        _logger.LogError("Failed to assign admin user to Admin role.");
                        foreach (var error in createResult.Errors)
                        {
                            _logger.LogError($"Error: {error.Description}");
                        }
                    }

                }
                else
                {
                    _logger.LogError("Failed to create admin user.");
                    foreach (var error in createResult.Errors)
                    {
                        _logger.LogError($"Error: {error.Description}");
                    }
                }
            }


            else
            {
                _logger.LogInformation("Admin user already exists.");
            }
            _logger.LogInformation("Admin user seeded successfully.");
        }
    }
}
