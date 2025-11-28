using GGone.API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    // localhost:5001/api/GGone
    [Route("api/[controller]")]
    [ApiController]
    public class GGoneController : ControllerBase
    {
        public GGoneController(GGoneDbContext dbContext) 
        {
            DbContext = dbContext;
        }

        public GGoneDbContext DbContext { get; }

        [HttpGet]
        public IActionResult GetAllGGone()
        {
            var allGGone = DbContext.Users.ToList();

            return Ok(allGGone);
        }
    }
}
