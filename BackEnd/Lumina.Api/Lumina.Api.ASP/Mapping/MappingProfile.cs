using AutoMapper;
using Lumina.Api.ASP.DTO.Auth;
using Lumina.Models;

namespace Lumina.Api.ASP.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.UserName) ? src.Email : src.UserName))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.LastLoginAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));
        }
    }
}
