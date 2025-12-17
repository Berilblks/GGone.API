using GGone.API.Business.Abstracts;
using GGone.API.Data;
using GGone.API.Models.BMI;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [ApiController] 
    [Route("api/[controller]")] 
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
    }
}
