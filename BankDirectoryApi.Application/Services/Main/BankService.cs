using AutoMapper;
using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankDirectoryApi.Application.DTOs.Core;
using BankDirectoryApi.Application.Interfaces.Main;
using BankDirectoryApi.Domain.Classes.Specifications;
using BankDirectoryApi.Common.Helpers;

namespace BankDirectoryApi.Application.Services.Main
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
        public async Task<IEnumerable<BankDTO>> GetAllBanksAsync(BankFilterDTO model,
            CancellationToken cancellationToken)
        {
            var spec = new Specification<Bank>()
            {
                Criteria = ExpressionFilterHelper.CreateFilter<Bank>(model),
                Orderings = OrderingHelper.GetOrderings<Bank>(model.OrderingInfo),
                IsPagingEnabled = model.PaginationInfo != null,
                PageNumber = model.PaginationInfo?.PageNumber,
                PageSize = model.PaginationInfo?.PageSize,
                AsNoTracking = true,
            };
           
            var banks = await _bankRepository.GetAllAsync(spec,cancellationToken);
            return _mapper.Map<IEnumerable<BankDTO>>(banks);
        }
        public async Task<IEnumerable<BankDTO>> GetAllBanksAsync(CancellationToken cancellationToken)
        {
           
            var banks = await _bankRepository.GetAllAsync(cancellationToken);
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
