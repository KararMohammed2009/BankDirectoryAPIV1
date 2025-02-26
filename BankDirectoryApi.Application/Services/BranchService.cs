using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services
{
    public class BranchService:IBranchService
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IBankRepository _bankRepository;
        public BranchService(IBranchRepository branchRepository,IBankRepository bankRepository)
        {
            _branchRepository = branchRepository;
            _bankRepository = bankRepository;
        }
        public async Task<IEnumerable<Branch>> GetAllBranchesAsync()
        {
            return await _branchRepository.GetAllAsync();
        }
        public async Task<Branch?> GetBranchByIdAsync(int id)
        {
            return await _branchRepository.GetByIdAsync(id);
        }

    

        public async Task AddBranchAsync(Branch Branch)
        {
            var bank = await _bankRepository.GetByIdAsync(Branch.BankId);

            if (bank == null)
            {
                throw new Exception($"Bank with ID {Branch.BankId} not found.");
            }
            Branch.Bank = null;

            await _branchRepository.AddAsync(Branch);
            await _branchRepository.SaveChangesAsync();
        }

        public async Task UpdateBranchAsync(Branch Branch)
        {
            _branchRepository.Update(Branch);
            await _branchRepository.SaveChangesAsync();
        }

        public async Task DeleteBranchAsync(int id)
        {
            var Branch = await _branchRepository.GetByIdAsync(id);
            if (Branch != null)
            {
                _branchRepository.Delete(Branch);
                await _branchRepository.SaveChangesAsync();
            }
        }
    }
}
