using BankDirectoryApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Infrastructure.Identity
{
    public class MyIdentityDbContext : IdentityDbContext<IdentityUser,IdentityRole,string>
    {
        public MyIdentityDbContext(DbContextOptions<MyIdentityDbContext> options)
            : base(options) { }

        // Add DbSet properties for other entities
       
    }
}
