using GGone.API.Business.Abstracts;
using GGone.API.Models;
using GGone.API.Models.AI;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIChatController : ControllerBase
    {
        private readonly IAIChatService _aiChatService;

        public AIChatController(IAIChatService aiChatService)
        {
            _aiChatService = aiChatService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AIChatRequest request)
        {
            var result = await _aiChatService.GetAiReply(request);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("test-gemini-key")]
        public IActionResult TestGeminiKey([FromServices] IConfiguration config)
        {
            var key = config["GeminiSettings:ApiKey"];

            return Ok(new
            {
                IsNull = key == null,
                IsEmpty = string.IsNullOrEmpty(key),
                Length = key?.Length
            });
        }
    }
}
