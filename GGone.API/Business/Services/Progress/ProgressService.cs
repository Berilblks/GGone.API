using GGone.API.Business.Abstracts;
using GGone.API.Data;
using GGone.API.Models;
using GGone.API.Models.Progress;
using Microsoft.EntityFrameworkCore;
using System;

namespace GGone.API.Business.Services.Progress
{
    public class ProgressService : IProgressService
    {
        private readonly GGoneDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ProgressService(GGoneDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<BaseResponse<ProgressOverviewResponse>> GetProgressOverview()
        {
            var userId = _currentUserService.UserId;
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null)
            {
                return new BaseResponse<ProgressOverviewResponse> { Success = false, Message = "User not found" };
            }

            // 1. Kilo Geçmişini Çek
            var history = await _context.WeightHistories
                .Where(w => w.UserId == userId)
                .OrderBy(w => w.Date)
                .ToListAsync();

            // Eğer hiç geçmiş yoksa, sadece şu anki kullanıcı kilosunu bir kayıt gibi kabul et
            if (!history.Any())
            {
                history.Add(new WeightHistory 
                { 
                    UserId = userId, 
                    Weight = user.Weight, 
                    Date = DateTime.UtcNow // veya User.CreatedAt 
                });
            }

            // En eski kayıt = Başlangıç kilosu
            var startWeight = history.First().Weight;
            // En yeni kayıt veya User.Weight = Güncel
            var currentWeight = user.Weight; 
            
            // Eğer User.Weight, son history kaydından farklıysa (manuel update yapılmışsa), onu da history'e dahil edebiliriz
            // Ama basit tutalım, WeightHistory tablosu esas olsun.
            // Fakat User.Weight en günceldir.
            
            var targetWeight = user.TargetWeight;

            // 2. İstatistikler
            var weightLost = startWeight - currentWeight;
            var remaining = currentWeight - targetWeight;
            
            // Progress Yüzdesi: (Verilen / (Başlangıç - Hedef)) * 100
            double percentage = 0;
            double totalToLose = startWeight - targetWeight;
            
            if (totalToLose > 0) // Zayıflama hedefi
            {
                percentage = (weightLost / totalToLose) * 100;
            }
            else if (totalToLose < 0) // Kilo alma hedefi (Start < Target)
            {
                // totalToLose negatif (-10 gibi). weightLost negatif olmalı (-5 kilo verdim değil aldım)
                // Formül: (Current - Start) / (Target - Start)
                percentage = ((currentWeight - startWeight) / (targetWeight - startWeight)) * 100;
            }

             // Değerleri sınırla (0-100)
            if (percentage < 0) percentage = 0;
            if (percentage > 100) percentage = 100;


            // 3. BMI Hesaplama
            // BMI = kg / (m^2)
            // Height cm geliyor, metreye çevir.
            double heightInMeters = user.Height / 100.0;
            double currentBmi = 0;
            if (heightInMeters > 0)
            {
                currentBmi = currentWeight / (heightInMeters * heightInMeters);
            }

            string bmiStatus = GetBmiStatus(currentBmi);

            // 4. History Listesi Hazırla
            var historyDto = history.Select(h => new WeightRecordDto
            {
                Date = h.Date,
                Weight = h.Weight,
                Bmi = (heightInMeters > 0) ? Math.Round(h.Weight / (heightInMeters * heightInMeters), 2) : 0
            }).ToList();

            // Eğer şu anki user.weight history'nin sonuncusundan farklıysa (ve tarih farklıysa) ekle
            // Basitlik adına: History listesini olduğu gibi dönüyoruz.

            return new BaseResponse<ProgressOverviewResponse>
            {
                Success = true,
                Data = new ProgressOverviewResponse
                {
                    CurrentWeight = currentWeight,
                    StartWeight = startWeight,
                    TargetWeight = targetWeight,
                    WeightLost = Math.Round(weightLost, 1),
                    RemainingWeight = Math.Round(remaining, 1),
                    ProgressPercentage = Math.Round(percentage, 1),
                    CurrentBmi = Math.Round(currentBmi, 2),
                    BmiStatus = bmiStatus,
                    History = historyDto,
                    CurrentStreak = user.ActiveDays // User modelindeki active days
                }
            };
        }

        private string GetBmiStatus(double bmi)
        {
            if (bmi < 18.5) return "Underweight";
            if (bmi < 24.9) return "Normal";
            if (bmi < 29.9) return "Overweight";
            return "Obese";
        }
    }
}
