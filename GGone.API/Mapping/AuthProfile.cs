using AutoMapper;
using GGone.API.Models.Auth;

namespace GGone.API.Mapping
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterRequest, User>()
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => new DateOnly(src.BirthYear, src.BirthMonth, src.BirthDay)));
            CreateMap<User, RegisterResponse>();
            CreateMap<User, ProfileResponse>();
        }
    }
}
