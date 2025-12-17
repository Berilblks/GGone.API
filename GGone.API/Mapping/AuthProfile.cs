using AutoMapper;
using GGone.API.Models.Auth;

namespace GGone.API.Mapping
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterRequest, User>();
            CreateMap<User, RegisterResponse>();
        }
    }
}
