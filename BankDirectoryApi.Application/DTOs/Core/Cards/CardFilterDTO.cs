using BankDirectoryApi.Domain.Attributes;
using BankDirectoryApi.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;
using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankDirectoryApi.Domain.Classes.Pagination;

namespace BankDirectoryApi.Application.DTOs.Core.Cards
{
    public class CardFilterDTO
    {
        [SwaggerSchema("The card name.", Nullable = true)]
        [Filter(FilterType.Contains, typeof(Card), nameof(Card.Name))]
        public string? CardName { get; set; }
        [SwaggerSchema("The card type.", Nullable = true)]
        [Filter(FilterType.Equals, typeof(Card), nameof(Card.Type))]
        public CardType? CardType { get; set; }
        [SwaggerSchema("The card annual fee.", Nullable = true)]
        [Filter(FilterType.Contains, typeof(Card), nameof(Card.AnnualFee))]
        public decimal? AnnualFee { get; set; }
        
        [SwaggerSchema("The bank ID of the card.", Nullable = true)]
        [Filter(FilterType.Equals, typeof(Card), nameof(Card.BankId))]
        public int? BankId { get; set; }
        [SwaggerSchema("The pagination information.", Nullable = true)]
        public PaginationInfo? PaginationInfo { get; set; }
        [SwaggerSchema("The ordering information.", Nullable = true)]
        public Dictionary<string, string>? OrderingInfo { get; set; }
    }
}
