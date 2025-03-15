using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces.Main
{
    public interface IATMService
    {
        Task<IEnumerable<ATM>> GetAllATMsAsync();
        Task<ATM?> GetATMByIdAsync(int id);
        Task AddATMAsync(ATM aTM);
        Task UpdateATMAsync(ATM aTM);
        Task DeleteATMAsync(int id);
    }
}
