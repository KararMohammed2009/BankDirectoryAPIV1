using AutoMapper;
using BankDirectoryApi.Application.DTOs.Core.Branch;
using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Mappings
{
    public class BranchProfile:Profile
    {
       public BranchProfile()
        {
            CreateMap<Branch, BranchDTO>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.BankId, opt => opt.MapFrom(src => src.BankId))
           .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(src => src.Address));

            CreateMap<BranchDTO, Branch>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.BankId, opt => opt.MapFrom(src => src.BankId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.FullAddress));
        }

    }
}
