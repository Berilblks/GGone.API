using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using GGone.API.Business.Abstracts;
using GGone.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GGone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WaterController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly GGoneDbContext _context; 
        private readonly ITaskService _taskService;

        public WaterController(
            ICurrentUserService currentUserService, 
            GGoneDbContext context,
            ITaskService taskService)
        {
            _currentUserService = currentUserService;
            _context = context;
            _taskService = taskService;
        }

        [HttpGet("Intake")]
        public async Task<IActionResult> GetIntake()
        {
            var userId = _currentUserService.UserId;
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null) return NotFound("User not found");

            // Reset counter if day changed
            if (user.LastWaterDate.Date != DateTime.Now.Date)
            {
                user.CurrentWaterIntake = 0;
                user.LastWaterDate = DateTime.Now;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            return Ok(user.CurrentWaterIntake);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateIntake([FromBody] int change)
        {
            var userId = _currentUserService.UserId;
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound("User not found");

            // Check day
            if (user.LastWaterDate.Date != DateTime.Now.Date)
            {
                user.CurrentWaterIntake = 0;
                user.LastWaterDate = DateTime.Now;
            }

            // Update value
            user.CurrentWaterIntake += change;
            if (user.CurrentWaterIntake < 0) user.CurrentWaterIntake = 0; // Cannot be negative
            
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // TARGET CHECK (8 Glasses)
            if (user.CurrentWaterIntake >= 8)
            {
                // Find "Water" task and complete it
                var tasksResponse = await _taskService.GetTodayTasks();
                if (tasksResponse.Data != null)
                {
                    // Find task with title containing "Water", "Su" or "bardak"
                    // Note: tasksResponse.Data is List<DailyTaskResponse>
                    var waterTask = tasksResponse.Data.FirstOrDefault(t => t.Title.Contains("Water") || t.Title.Contains("Su") || t.Title.Contains("bardak"));
                
                    if (waterTask != null && !waterTask.IsCompleted)
                    {
                        var toggleRequest = new GGone.API.Models.Tasks.ToggleCompletionRequest 
                        { 
                            TaskId = waterTask.Id, 
                            IsCompleted = true 
                        };
                        await _taskService.ToggleTaskCompletion(toggleRequest);
                    }
                }
            }
            return Ok(user.CurrentWaterIntake);
        }
    }
}
