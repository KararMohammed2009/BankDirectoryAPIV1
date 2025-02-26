using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankDirectoryApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankDirectoryApi.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Bank> Banks { get; set; } = null!;
        public DbSet<ATM> ATMs { get; set; } = null!;
        public DbSet<Card> Cards { get; set; } = null!;
        public DbSet<Branch> Branches { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add any custom configurations here

            modelBuilder.Entity<Branch>()
             .OwnsOne(b => b.Address);
        }
    }
}
