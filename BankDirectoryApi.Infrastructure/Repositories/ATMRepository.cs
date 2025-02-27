
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Infrastructure.Repositories
{
    public class ATMRepository:Repository<ATM>,IATMRepository
    {
        private readonly ApplicationDbContext _context;
        public ATMRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}
