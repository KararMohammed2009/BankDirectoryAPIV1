using AutoMapper;
using BankDirectoryApi.Application.DTOs.Related.UserManagement;
using BankDirectoryApi.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Application.Mappings.Related
{
    /// <summary>
    /// This class is responsible for mapping between ApplicationUser and related DTOs.
    /// </summary>
    public class ApplicationUserProfile:Profile
    {
        /// <summary>
        /// Constructor for ApplicationUserProfile.
        /// </summary>
        public ApplicationUserProfile()
        {
            /// Mapping ApplicationUser to UserDTO
            CreateMap<ApplicationUser, UserDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
            /// Mapping UserDTO to ApplicationUser
            CreateMap<UserDTO, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
            /// Mapping ApplicationUser to UpdateUserDTO
            CreateMap<ApplicationUser, UpdateUserDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
            /// Mapping UpdateUserDTO to ApplicationUser
            CreateMap<ApplicationUser, RegisterUserDTO>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
               .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled));
            CreateMap<RegisterUserDTO,ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled));



        }
    }
}
