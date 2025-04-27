using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankDirectoryApi.Infrastructure.Data
{
    /// <summary>
    /// ApplicationDbContext is the main database context for the application that inherits from IdentityDbContext (which provides identity features).
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
       
    
        public DbSet<Bank> Banks { get; set; } = null!;
        public DbSet<ATM> ATMs { get; set; } = null!;
        public DbSet<Card> Cards { get; set; } = null!;
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;




        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
        public ApplicationDbContext()
           
        { }
        /// <summary>
        /// This method is called when the model for a derived context is being created.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add any custom configurations here  
            ConfigureIdentity(modelBuilder); // Call to configure Identity
            ConfigureEntities(modelBuilder);
        }
        /// <summary>
        /// This method configures the Identity-related entities and their properties.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private static void ConfigureIdentity(ModelBuilder modelBuilder)
        {
            // Explicitly configure Identity
         

            modelBuilder.Entity<ApplicationUser>(user =>
            {
                user.Property(u => u.CreationDate).IsRequired();
            });
           

        }
        /// <summary>
        /// This method configures the entities and their relationships.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private static void ConfigureEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Branch>()
                .OwnsOne(b => b.Address);
            modelBuilder.Entity<Branch>()
                .OwnsOne(b => b.GeoCoordinate);
            modelBuilder.Entity<Bank>()
                .OwnsOne(b => b.Address);
            modelBuilder.Entity<Bank>()
                .OwnsOne(b => b.GeoCoordinate);
            modelBuilder.Entity<ATM>()
                .OwnsOne(a => a.GeoCoordinate);
            modelBuilder.Entity<Card>()
                .Property(c => c.AnnualFee)
                .HasColumnType("decimal(18,2)"); // Specify the precision and scale for decimal type

            // Configure the relationship for RefreshToken
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()  
                .HasForeignKey(rt => rt.UserId);
        }
    }
}
