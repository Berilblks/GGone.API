using GGone.API.Models;
using GGone.API.Models.Tasks;


namespace GGone.API.Business.Abstracts
{
    public interface ITaskService
    {
        Task<BaseResponse<List<DailyTaskResponse>>> GetTodayTasks();
        Task<BaseResponse<bool>> ToggleTaskCompletion(ToggleCompletionRequest request);

        Task<TaskItem> CheckAndAwardBadges(string taskId);
    }
}
