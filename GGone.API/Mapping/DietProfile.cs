using AutoMapper;
using GGone.API.Models.Diets;

namespace GGone.API.Mapping
{
    public class DietProfile : Profile
    {
        public DietProfile() 
        {
            CreateMap<WeeklyDietPlan, WeeklyDietPlan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); 

            CreateMap<DietDay, DietDay>();
        }
    }
}
