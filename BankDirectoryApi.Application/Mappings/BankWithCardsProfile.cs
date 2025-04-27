using AutoMapper;
using BankDirectoryApi.Application.DTOs.Core.Banks;
using BankDirectoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Mappings
{
    public class BankWithCardsProfile : Profile
    {
        public BankWithCardsProfile()
        {
            // Bank -> BankWithCardsDTO
            CreateMap<Bank, BankWithCardsDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CustomerSupportNumber, opt => opt.MapFrom(src => src.CustomerSupportNumber))
                .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Website))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.GeoCoordinate, opt => opt.MapFrom(src => src.GeoCoordinate))
                .ForMember(dest => dest.Cards, opt => opt.MapFrom(src => src.Cards)); // Maps nested cards
            // DTOs back to Entities (For Updates)
            CreateMap<BankWithCardsDTO, Bank>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CustomerSupportNumber, opt => opt.MapFrom(src => src.CustomerSupportNumber))
                .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Website))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.GeoCoordinate, opt => opt.MapFrom(src => src.GeoCoordinate))
                .ForMember(dest => dest.Cards, opt => opt.MapFrom(src => src.Cards)); // Maps nested cards
        }
    }
    }
