using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services
{
    public class ATMService:IATMService
    {
        private readonly IATMRepository _aTMRepository;
        public ATMService(IATMRepository aTMRepository)
        {
            _aTMRepository = aTMRepository;
        }
        public async Task<IEnumerable<ATM>> GetAllATMsAsync()
        {
            return await _aTMRepository.GetAllAsync();
        }
        public async Task<ATM?> GetATMByIdAsync(int id)
        {
            return await _aTMRepository.GetByIdAsync(id);
        }

      

        public async Task AddATMAsync(ATM ATM)
        {
            await _aTMRepository.AddAsync(ATM);
            await _aTMRepository.SaveChangesAsync();
        }

        public async Task UpdateATMAsync(ATM ATM)
        {
            _aTMRepository.Update(ATM);
            await _aTMRepository.SaveChangesAsync();
        }

        public async Task DeleteATMAsync(int id)
        {
            var ATM = await _aTMRepository.GetByIdAsync(id);
            if (ATM != null)
            {
                _aTMRepository.Delete(ATM);
                await _aTMRepository.SaveChangesAsync();
            }
        }
    }
}
