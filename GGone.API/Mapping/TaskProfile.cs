using AutoMapper;
using GGone.API.Models.Tasks;

namespace GGone.API.Mapping
{
    public class TaskProfile : Profile
    {
        public TaskProfile() {
            CreateMap<TaskItem, DailyTaskResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TaskId))
                .ForMember(dest => dest.IsCompleted, opt => opt.Ignore());
        }
    }
}
