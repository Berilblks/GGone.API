using GGone.API.Business.Abstracts;
using GGone.API.Data;
using GGone.API.Models.BMI;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [ApiController] 
    [Route("api/[controller]")] 
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class BmiController : ControllerBase
    {
        private readonly IBmiService _bmiService;
        private readonly GGoneDbContext _context;

        // Dependency Injection ile servisi içeri alıyoruz
        public BmiController(IBmiService bmiService, GGoneDbContext context)
        {
            _bmiService = bmiService;
            _context = context;
        }

        /// Flutter'dan gelen BMI hesaplama ve kaydetme isteğini yönetir.
        [HttpPost("calculate")] 
        public async Task<IActionResult> CalculateAndSave([FromBody] CreateBmiRequest request)
        {
            // 1. Gelen verinin boş olup olmadığını kontrol et (Opsiyonel Validation)
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                // 2. Servisi çağır ve sonucu al
                var result = await _bmiService.CalculateAndSaveAsync(request);

                // 3. Başarılı sonucu (BmiResponse) Flutter'a geri dön
                return Ok(result);
            }
            catch (Exception ex)
            {
                // 4. Hata durumunda loglama yapılabilir ve hata mesajı dönülür
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetBmiHistory()
        {
            var result = await _bmiService.GetBmiHistory();

            if (result == null || !result.Any())
                return NotFound("Henüz kilo kaydı bulunamadı.");

            return Ok(result);
        }

        [HttpGet("streak")]
        public async Task<IActionResult> GetUserStreak()
        {
            try
            {
                // 1. Servis üzerinden hesaplanan streak sayısını al
                var streakCount = await _bmiService.GetUserStreak();

                // 2. Başarılı şekilde Flutter'a dön
                return Ok(new { streakCount = streakCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Streak hesaplanırken bir hata oluştu: {ex.Message}");
            }
        }
    }
}
