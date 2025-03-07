using BankDirectoryApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankDirectoryApi.Domain.Entities;

namespace BankDirectoryApi.IntegrationTestss.Data
{
    public static class SeedData
    {
        public static void PopulateTestData(ApplicationDbContext db) // Replace ApplicationDbContext with your context
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }

            // Check if data already exists to avoid duplicate entries.
            if (!db.Banks.Any())
            {
                // Add sample accounts
                db.Banks.AddRange(
                    new Bank { Name = "Bank of America" },
                    new Bank { Name = "Chase" }
                );
            }

            if (!db.Cards.Any())
            {
                db.Cards.AddRange(
                    new Card { Name = "1234567890123456",  BankId = 1 },
                    new Card { Name = "1234567890123457", BankId = 2 }
                );
            }

            // Add other entities as needed (e.g., Transactions, Customers)

            db.SaveChanges();
        }
    }
}
