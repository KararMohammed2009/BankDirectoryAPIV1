using BankDirectoryApi.Application.DTOs;
using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces
{
    public interface IBankService
    {
        Task<IEnumerable<BankDTO>> GetAllBanksAsync();
        Task<BankDTO?> GetBankByIdAsync(int id);
        Task<BankWithBranchesDTO?> GetBankWithBranchesAsync(int id);
        Task AddBankAsync(BankDTO bank);
        Task UpdateBankAsync(BankDTO bank);
        Task DeleteBankAsync(int id);
    }
}
