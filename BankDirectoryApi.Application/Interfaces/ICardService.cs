using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Interfaces
{
    public interface ICardService
    {
        Task<IEnumerable<Card>> GetAllCardsAsync();
        Task<Card?> GetCardByIdAsync(int id);
        Task AddCardAsync(Card cards);
        Task UpdateCardAsync(Card cards);
        Task DeleteCardAsync(int id);
    }
}
