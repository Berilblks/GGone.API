using AutoMapper;
using GGone.API.Models.Auth;
using GGone.API.Models.Friends;

namespace GGone.API.Mapping
{
    public class FriendProfile : Profile
    {
        public FriendProfile()
        {
            CreateMap<User, FriendResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.ProfilePhotoUrl))
                // Level ve Steps mantığı
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => "1"))
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => 0));
        }
    }
}
