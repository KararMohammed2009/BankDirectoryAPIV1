using AutoMapper;
using BankDirectoryApi.Application.DTOs;
using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services
{
    public class BankService:IBankService
    {
        private readonly IMapper _mapper;
        private readonly IBankRepository _bankRepository;
        public BankService(IBankRepository bankRepository,IMapper mapper)
        {
            _bankRepository = bankRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<BankDTO>> GetAllBanksAsync()
        {
            var banks =  await _bankRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BankDTO>>(banks);
        }
        public async Task<BankDTO?> GetBankByIdAsync(int id)
        {
            var bank =  await _bankRepository.GetByIdAsync(id);
            return _mapper.Map<BankDTO>(bank);
        }

        public async Task<BankWithBranchesDTO?> GetBankWithBranchesAsync(int id)
        {
            var banksWithBranches = await _bankRepository.GetBankWithBranchesAsync(id);
            return _mapper.Map<BankWithBranchesDTO>(banksWithBranches);
        }

        public async Task AddBankAsync(BankDTO bankdto)
        {
            var bank = _mapper.Map<Bank>(bankdto);
            await _bankRepository.AddAsync(bank);
            await _bankRepository.SaveChangesAsync();
        }

        public async Task UpdateBankAsync(BankDTO bankdto)
        {
            var bank = _mapper.Map<Bank>(bankdto);
            _bankRepository.Update(bank);
            await _bankRepository.SaveChangesAsync();
        }

        public async Task DeleteBankAsync(int id)
        {
            var bank = await _bankRepository.GetByIdAsync(id);
            if (bank != null)
            {
                _bankRepository.Delete(bank);
                await _bankRepository.SaveChangesAsync();
            }
        }
    }
}
