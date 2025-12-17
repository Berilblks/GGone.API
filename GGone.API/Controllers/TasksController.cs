using GGone.API.Business.Abstracts;
using GGone.API.Models;
using GGone.API.Models.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Bugünün görevlerini listeler ve tamamlanma durumlarını döner.
        /// </summary>
        [HttpGet("today")]
        public async Task<ActionResult<BaseResponse<List<DailyTaskResponse>>>> GetTodayTasks()
        {
            var response = await _taskService.GetTodayTasks();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// Bir görevin tamamlanma durumunu değiştirir (Tamamlandı/Tamamlanmadı).
        [HttpPost("toggle-completion")]
        public async Task<ActionResult<BaseResponse<bool>>> ToggleCompletion([FromBody] ToggleCompletionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<bool>.Fail("Geçersiz istek verisi."));
            }

            var response = await _taskService.ToggleTaskCompletion(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            // Rozet kazanma kontrolü gibi ek işlemler burada CheckAndAwardBadges ile tetiklenebilir
            if (request.IsCompleted)
            {
                await _taskService.CheckAndAwardBadges(request.TaskId);
            }

            return Ok(response);
        }
    }
}
