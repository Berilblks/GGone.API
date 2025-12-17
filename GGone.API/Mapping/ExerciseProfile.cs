using AutoMapper;
using GGone.API.Models.Exercises;


namespace GGone.API.Mapping
{
    public class ExerciseProfile : Profile
    {
        public ExerciseProfile() 
        {
            CreateMap<Exercise, ExerciseResponse>();
        }
    }
}
