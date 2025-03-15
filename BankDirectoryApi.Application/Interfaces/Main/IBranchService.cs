using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Main
{
    public interface IBranchService
    {
        Task<IEnumerable<Branch>> GetAllBranchesAsync();
        Task<Branch?> GetBranchByIdAsync(int id);
        Task AddBranchAsync(Branch Branchs);
        Task UpdateBranchAsync(Branch Branchs);
        Task DeleteBranchAsync(int id);
    }
}
