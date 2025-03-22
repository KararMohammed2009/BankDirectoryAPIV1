using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Infrastructure.Data
{
    public class ApplicationDbContext : MyIdentityDbContext
    {
        public DbSet<Bank> Banks { get; set; } = null!;
        public DbSet<ATM> ATMs { get; set; } = null!;
        public DbSet<Card> Cards { get; set; } = null!;
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<MyIdentityDbContext> options)
            : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add any custom configurations here
            // Add DbSet properties for other entities

            modelBuilder.Entity<Branch>()
             .OwnsOne(b => b.Address);
        }


    }
}
