using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<Bank> Banks { get; set; } = null!;
        public DbSet<ATM> ATMs { get; set; } = null!;
        public DbSet<Card> Cards { get; set; } = null!;
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        // Fix for CS7036: Pass the 'options' parameter to the base class constructor.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add any custom configurations here  
            ConfigureIdentity(modelBuilder); // Call to configure Identity
            ConfigureEntities(modelBuilder);
        }
        private static void ConfigureIdentity(ModelBuilder modelBuilder)
        {
            // Explicitly configure ApplicationUser
            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.CreationDate).IsRequired();
                // Add any other specific ApplicationUser configurations here
            });

            // You might not need to reconfigure IdentityRole, but you can if needed
            modelBuilder.Entity<ApplicationRole>(b =>
            {
                b.HasKey(r => r.Id);
                // Add any specific ApplicationRole configurations here
            });
        }

        private static void ConfigureEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Branch>()
                .OwnsOne(b => b.Address);

            // Configure the relationship for RefreshToken
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()  // No explicit collection navigation property in ApplicationUser
                .HasForeignKey(rt => rt.UserId);
        }
    }
}
