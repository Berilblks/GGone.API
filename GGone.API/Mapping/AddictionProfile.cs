using AutoMapper;
using GGone.API.Models.Addiction;
using GGone.API.Models.Addictions;

namespace GGone.API.Mapping
{
    public class AddictionProfile : Profile
    {
        public AddictionProfile()
        {
            CreateMap<AddAddictionRequest, Addiction>()
                .ForMember(dest => dest.AddictionType, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Id, op => op.Ignore());

            CreateMap<Addiction, CounterResponse>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.AddictionType))
                .ForMember(dest => dest.DaysClean, opt => opt.Ignore());
        }
    }
}
