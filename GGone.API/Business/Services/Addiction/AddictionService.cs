using GGone.API.Business.Abstracts;
using GGone.API.Data;
using GGone.API.Models;
using GGone.API.Models.Addictions;
using GGone.API.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace GGone.API.Business.Services.Addiction
{
    public class AddictionService : IAddictionService
    {
        private readonly GGoneDbContext _context;
        public AddictionService(GGoneDbContext context)
        {
            _context = context;
        }

        // Kullanıcının belirli bir bağımlılığı eklemesini sağlar
        // 1. AddUserAddictionAsync (Güncellendi)
        public async Task<BaseResponse<Models.Addiction.Addiction>> AddUserAddictionAsync(AddAddictionRequest request)
        {
            try
            {
                var existingAddiction = await _context.Addictions
                    .FirstOrDefaultAsync(a => a.UserId == request.UserId && a.AddictionType == request.Type); // request.UserId kullanıldı

                if (existingAddiction != null)
                {
                    existingAddiction.QuitDate = request.QuitDate;
                    existingAddiction.DailyConsumption = request.DailyConsumption;
                    existingAddiction.UnitPrice = request.UnitPrice;
                    existingAddiction.LastConsumptionDate = DateTime.UtcNow;
                }
                else
                {
                    existingAddiction = new Models.Addiction.Addiction
                    {
                        UserId = request.UserId,
                        AddictionType = request.Type,
                        QuitDate = request.QuitDate,
                        DailyConsumption = request.DailyConsumption,
                        UnitPrice = request.UnitPrice,
                        LastConsumptionDate = DateTime.UtcNow
                    };
                    _context.Addictions.Add(existingAddiction);
                }

                await _context.SaveChangesAsync();

                return new BaseResponse<Models.Addiction.Addiction> { Success = true, Data = existingAddiction };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Models.Addiction.Addiction> { Success = false, Message = $"Addiction update failed: {ex.Message}" };
            }
        }

        // Günlük durum kontrolü
        public async Task<BaseResponse<List<Models.Addiction.Addiction>>> GetUserAddictionsAsync(GetAddictionRequest request)
        {
            try
            {
                var addictions = await _context.Addictions
                    .Where(a => a.UserId == request.UserId)
                    .ToListAsync();

                return new BaseResponse<List<Models.Addiction.Addiction>> { Success = true, Data = addictions };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Models.Addiction.Addiction>> { Success = false, Message = $"Failed to retrieve addictions: {ex.Message}" };
            }
        }

        // Nüksetme durumunu kaydeder ve sayacı sıfırlar
        public async Task<BaseResponse<object>> QuitDateAsync(QuitDateRequest request)
        {
            try
            {
                var addiction = await _context.Addictions
                    .FirstOrDefaultAsync(a => a.UserId == request.UserId && a.AddictionType == request.Type);

                if (addiction == null)
                {
                    return new BaseResponse<object> { Success = false, Message = "Addiction record not found." };
                }

                if (request.DidConsume)
                {
                    addiction.QuitDate = DateTime.UtcNow;
                    addiction.LastConsumptionDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return new BaseResponse<object> { Success = true, Message = "Relapse recorded and counter reset." };
            }
            catch (Exception ex)
            {
                return new BaseResponse<object> { Success = false, Message = $"Relapse failed: {ex.Message}" };
            }
        }

        // 4. GetDependencyCounterAsync (Güncellendi)
        public async Task<BaseResponse<CounterResponse>> GetDependencyCounterAsync(GetCounterRequest request)
        {
            var addiction = await _context.Addictions
                .FirstOrDefaultAsync(a => a.UserId == request.UserId && a.AddictionType == request.Type);

            if (addiction == null)
            {
                return new BaseResponse<CounterResponse>
                {
                    Success = true, // Kayıt olmaması hata değil, veri yok demektir.
                    Data = new CounterResponse
                    {
                        UserId = request.UserId,
                        Type = request.Type,
                        DaysClean = 0,
                        QuitDate = DateTime.MinValue,
                        LastConsumptionDate = DateTime.MinValue,
                        DailyConsumption = 0,
                        UnitPrice = 0
                    }
                };
            }

            var daysClean = (int)(DateTime.UtcNow.Date - addiction.QuitDate).TotalDays;

            var response = new CounterResponse
            {
                UserId = request.UserId,
                Type = addiction.AddictionType,
                DaysClean = daysClean < 0 ? 0 : daysClean,
                QuitDate = addiction.QuitDate,
                LastConsumptionDate = addiction.LastConsumptionDate,
                DailyConsumption = addiction.DailyConsumption,
                UnitPrice = addiction.UnitPrice
            };

            return new BaseResponse<CounterResponse> { Success = true, Data = response };
        }

        // 5. CheckDailyStatusAsync (Güncellendi)
        public async Task<BaseResponse<bool>> CheckDailyStatusAsync(GetAddictionRequest request)
        {
            try
            {
                var hasRecord = await _context.Addictions
                    .AnyAsync(r => r.UserId == request.UserId);

                return new BaseResponse<bool> { Success = true, Data = hasRecord };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool> { Success = false, Message = $"Status check failed: {ex.Message}" };
            }
        }
    }
}
