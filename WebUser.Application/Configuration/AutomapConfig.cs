using AutoMapper;
using WebUser.Application.DTOs.UserDTO;
using WebUser.Domain.Entities;

namespace WebUser.Application.Configuration
{
    public class AutomapConfig : Profile
    {
        public AutomapConfig()
        {
            CreateMap<ApplicationUser, UserModelDTO>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.PasswordHash))
                .ReverseMap();
            CreateMap<ApplicationUser, UpdatedUserDTO>()
                .ReverseMap();
        }
    }
}
