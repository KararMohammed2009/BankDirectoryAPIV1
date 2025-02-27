using BankDirectoryApi.Application.Interfaces;
using BankDirectoryApi.Domain.Interfaces;
using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Services
{
    public class CardService:ICardService
    {
        private readonly ICardRepository _cardRepository;
        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }
        public async Task<IEnumerable<Card>> GetAllCardsAsync()
        {
            return await _cardRepository.GetAllAsync();
        }
        public async Task<Card?> GetCardByIdAsync(int id)
        {
            return await _cardRepository.GetByIdAsync(id);
        }



        public async Task AddCardAsync(Card Card)
        {
            await _cardRepository.AddAsync(Card);
            await _cardRepository.SaveChangesAsync();
        }

        public async Task UpdateCardAsync(Card Card)
        {
            _cardRepository.Update(Card);
            await _cardRepository.SaveChangesAsync();
        }

        public async Task DeleteCardAsync(int id)
        {
            var Card = await _cardRepository.GetByIdAsync(id);
            if (Card != null)
            {
                _cardRepository.Delete(Card);
                await _cardRepository.SaveChangesAsync();
            }
        }
    }
}
