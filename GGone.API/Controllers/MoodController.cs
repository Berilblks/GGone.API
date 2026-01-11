using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GGone.API.Data;
using GGone.API.Business.Abstracts;
using System;
using System.Threading.Tasks;

namespace GGone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoodController : ControllerBase
    {
        private readonly GGoneDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public MoodController(GGoneDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        [HttpGet("Current")]
        public async Task<IActionResult> GetCurrentMood()
        {
            var userId = _currentUserService.UserId;
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            // Reset mood if it's a new day
            if (user.LastMoodDate.Date != DateTime.Now.Date)
            {
                return Ok("");
            }
            return Ok(user.CurrentMood);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateMood([FromBody] string mood)
        {
            var userId = _currentUserService.UserId;
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.CurrentMood = mood;
            user.LastMoodDate = DateTime.Now;
            
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok(user.CurrentMood);
        }
    }
}
