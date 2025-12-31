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

        public async Task<List<BmiResponse>> GetBmiHistory()
        {
            var userId = _currentUserService.UserId; 

            var records = await _context.UserHealthRecords
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.CreatedAt) // Grafikte zaman dizini için sıralama önemlidir
                .ToListAsync();

            return _mapper.Map<List<BmiResponse>>(records); //
        }

        public async Task<BmiResponse?> GetLatestBmiByUserId()
        {
            var userId = _currentUserService.UserId;
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

        public async Task<int> GetUserStreak()
        {
            var userId = _currentUserService.UserId; 

            // Kullanıcının benzersiz kayıt tarihlerini alıyoruz
            var dates = await _context.UserHealthRecords
                .Where(x => x.UserId == userId) 
                .OrderByDescending(x => x.CreatedAt) 
                .Select(x => x.CreatedAt.Date)
                .Distinct()
                .ToListAsync();

            if (!dates.Any()) return 0;

            int streak = 0;
            DateTime compareDate = DateTime.Now.Date;

            // Eğer bugün kayıt yoksa, düne bakarak başla (streak bozulmasın diye)
            if (dates[0] != compareDate && dates[0] != compareDate.AddDays(-1))
                return 0;

            if (dates[0] == compareDate.AddDays(-1))
                compareDate = compareDate.AddDays(-1);

            foreach (var date in dates)
            {
                if (date == compareDate)
                {
                    streak++;
                    compareDate = compareDate.AddDays(-1);
                }
                else break;
            }
            return streak;
        }

        public async Task<int> GetUserStreakAsync()
        {
            var userId = _currentUserService.UserId;

            // Kullanıcının benzersiz kayıt tarihlerini (sadece gün bazında) büyükten küçüğe çekiyoruz
            var dates = await _context.UserHealthRecords
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.CreatedAt.Date)
                .Distinct()
                .ToListAsync();

            if (!dates.Any()) return 0;

            int streak = 0;
            DateTime today = DateTime.Now.Date;
            DateTime compareDate = today;

            // Eğer bugün kayıt yoksa, streak bozulmuş olabilir mi? 
            // Genelde streak dün kayıt varsa devam eder, bugün henüz girilmemiş olabilir.
            if (dates[0] != today && dates[0] != today.AddDays(-1))
                return 0;

            foreach (var date in dates)
            {
                if (date == compareDate || date == compareDate.AddDays(-1))
                {
                    streak++;
                    compareDate = date;
                }
                else
                {
                    break;
                }
            }
            return streak;
        }
    }
}
