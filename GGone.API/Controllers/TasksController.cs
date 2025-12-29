using GGone.API.Business.Abstracts;
using GGone.API.Models;
using GGone.API.Models.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ICurrentUserService _currentUserService;

        public TasksController(ITaskService taskService, ICurrentUserService currentUserService)
        {
            _taskService = taskService;
            _currentUserService = currentUserService;
        }

        /// Gets today's tasks for the current user
        [HttpGet("today")]
        public async Task<ActionResult<BaseResponse<List<DailyTaskResponse>>>> GetTodayTasks()
        {
            var response = await _taskService.GetTodayTasks(_currentUserService.UserId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// Toggles task completion status
        [HttpPost("toggle-completion")]
        public async Task<ActionResult<BaseResponse<bool>>> ToggleCompletion(
            [FromBody] ToggleCompletionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(BaseResponse<bool>.Fail("Geçersiz veri."));

            // Servis metodu hem request hem de UserId bekliyordu
            var response = await _taskService.ToggleTaskCompletion(request, _currentUserService.UserId);

            return response.Success ? Ok(response) : BadRequest(response);
        }
        
    }
}
