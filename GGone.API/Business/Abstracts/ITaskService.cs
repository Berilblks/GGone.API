using GGone.API.Models;
using GGone.API.Models.Tasks;


namespace GGone.API.Business.Abstracts
{
    public interface ITaskService
    {
        Task<BaseResponse<List<DailyTaskResponse>>> GetTodayTasks(int userId);
        Task<BaseResponse<bool>> ToggleTaskCompletion(ToggleCompletionRequest request, int userId);


        Task<TaskItem> CheckAndAwardBadges(string taskId);
    }
}
