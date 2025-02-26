using AutoMapper;
using BankDirectoryApi.Application.DTOs;
using BankDirectoryApi.Domain.Entities;

namespace BankDirectoryApi.Application.Mappings
{
    public class BankProfile:Profile
    {
        public BankProfile()
        {
            CreateMap<Bank, BankDTO>().ReverseMap(); // Bidirectional Mapping

        }
    }
}
