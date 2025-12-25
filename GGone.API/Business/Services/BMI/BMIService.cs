using AutoMapper;
using GGone.API.Business.Abstracts;
using GGone.API.Business.Rules;
using GGone.API.Data;
using GGone.API.Models.BMI;
using Microsoft.EntityFrameworkCore;

namespace GGone.API.Business.Services.BMI
{
    public class BMIService : IBmiService
    {
        private readonly GGoneDbContext _context; // Veritabanı bağlantısı
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public BMIService(GGoneDbContext context, IMapper mapper, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<BmiResponse> CalculateAndSaveAsync(CreateBmiRequest request)
        {
            // 1. DTO'yu Entity'ye çevir
            var record = _mapper.Map<UserHealthRecord>(request);

            // 2. BMI Hesapla
            double heightInMeters = record.Height / 100.0;
            record.BmiResult = Math.Round(record.Weight / (heightInMeters * heightInMeters), 2);
            record.CreatedAt = DateTime.Now;
            record.UserId = _currentUserService.UserId;

            // 3. Veritabanına kaydet
            _context.UserHealthRecords.Add(record);
            await _context.SaveChangesAsync();

            // 4. Sonucu Response DTO'suna çevir
            var response = _mapper.Map<BmiResponse>(record);

            response.Status = BmiRules.GetStatus(record.BmiResult);

            return response;
        }

        public async Task<BmiResponse?> GetLatestBmiByUserId(int userId)
        {
            var latestRecord = await _context.UserHealthRecords
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestRecord == null)
                return null;

            var response = _mapper.Map<BmiResponse>(latestRecord);
            response.Status = BmiRules.GetStatus(latestRecord.BmiResult);

            return response;
        }
    }
}
