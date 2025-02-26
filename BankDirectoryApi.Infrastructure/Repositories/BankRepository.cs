using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Domain.Entities;
using BankDirectoryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Infrastructure.Repositories
{
    public class BankRepository:Repository<Bank>,IBankRepository
    {
        private readonly ApplicationDbContext _context;
        public BankRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Bank?> GetBankWithBranchesAsync(int bankId)
        {
            return await _context.Banks
                .Include(b => b.Branches)
                .FirstOrDefaultAsync(b => b.Id == bankId);
        }
    }
}
