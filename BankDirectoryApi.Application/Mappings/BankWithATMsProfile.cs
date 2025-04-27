using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping between Bank and BankWithATMsDTO.
    /// </summary>
    public class BankWithATMsProfile : AutoMapper.Profile
    {
        public BankWithATMsProfile()
        {
            // Bank -> BankWithATMsDTO
            CreateMap<Domain.Entities.Bank, Application.DTOs.Core.Banks.BankWithATMsDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CustomerSupportNumber, opt => opt.MapFrom(src => src.CustomerSupportNumber))
                .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Website))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.GeoCoordinate, opt => opt.MapFrom(src => src.GeoCoordinate))
                .ForMember(dest => dest.ATMs, opt => opt.MapFrom(src => src.ATMs)); // Maps nested ATMs
            // DTOs back to Entities (For Updates)
            CreateMap<Application.DTOs.Core.Banks.BankWithATMsDTO, Domain.Entities.Bank>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CustomerSupportNumber, opt => opt.MapFrom(src => src.CustomerSupportNumber))
                .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Website))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.GeoCoordinate, opt => opt.MapFrom(src => src.GeoCoordinate))
                .ForMember(dest => dest.ATMs, opt => opt.MapFrom(src => src.ATMs)); // Maps nested ATMs
        }
    }
    }
