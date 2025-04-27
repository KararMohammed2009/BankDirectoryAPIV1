using AutoMapper;
using BankDirectoryApi.Application.DTOs.Core.Banks;
using BankDirectoryApi.Domain.Entities;

namespace BankDirectoryApi.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping between Bank and BankDTO.
    /// </summary>
    public class BankProfile:Profile
    {
        public BankProfile()
        {
            CreateMap<Bank, BankDTO>().ReverseMap(); // Bidirectional Mapping

        }
    }
}
