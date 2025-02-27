using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.Interfaces
{
    public interface IBankRepository:IRepository<Bank>
    {
        Task<Bank?> GetBankWithBranchesAsync(int bankId);
    }
}
