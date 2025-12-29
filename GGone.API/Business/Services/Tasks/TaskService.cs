using AutoMapper;
using GGone.API.Business.Abstracts;
using GGone.API.Data;
using GGone.API.Models;
using GGone.API.Models.Auth;
using GGone.API.Models.Enum;
using GGone.API.Models.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GGone.API.Business.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly GGoneDbContext _context; // Veritabanı bağlamınız
        private readonly IMapper _mapper;

        public TaskService(GGoneDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TaskItem> CheckAndAwardBadges(string taskId)
        {
            // Rozet mantığı buraya eklenebilir
            return null;
        }

        public async Task<BaseResponse<List<DailyTaskResponse>>> GetTodayTasks(int UserId)
        {
            // Kullanıcının bağımlılık bilgilerini çek (Örnek: Alkol mü Sigara mı?)
            var userAddiction = await _context.Addictions.FirstOrDefaultAsync(a => a.UserId == UserId);

            // Tüm aktif görevleri getir
            var allTasks = await _context.TaskItems.Where(x => x.IsActive).ToListAsync();

            // Filtreleme Mantığı: Sadece kullanıcının bağımlılığına uyan veya bağımlılık dışı görevleri al
            var filteredTasks = allTasks.Where(t =>
            {
                if (t.Category != "Addiction") return true; // Diğer tüm kategoriler gelsin

                if (userAddiction == null) return false; // Bağımlılığı yoksa bu kategoriyi gösterme

                // userAddiction.Type yerine userAddiction.AddictionType kullanın
                if (userAddiction.AddictionType == AddictionType.Smoking && t.TaskId == "addict_smoking") return true;
                if (userAddiction.AddictionType == AddictionType.Alcohol && t.TaskId == "addict_alcohol") return true;

                return false;
            }).ToList();

            // Logları kontrol et ve Map yap
            var today = DateTime.UtcNow.Date;
            var log = await _context.DailyTaskLogs.FirstOrDefaultAsync(x => x.Date.Date == today && x.UserId == UserId); 

            var response = _mapper.Map<List<DailyTaskResponse>>(filteredTasks);

            if (log != null)
            {
                foreach (var item in response)
                {
                    item.IsCompleted = log.CompletedTaskIds.Contains(item.Id);
                }
            }

            return BaseResponse<List<DailyTaskResponse>>.Ok(response);
        }

        public async Task<BaseResponse<bool>> ToggleTaskCompletion(ToggleCompletionRequest request, int userId)
        {
            var today = DateTime.UtcNow.Date;

            var log = await _context.DailyTaskLogs
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.Date == today);

            // Bugün için log yoksa oluştur
            if (log == null)
            {
                log = new DailyTaskLog
                {
                    UserId = userId,
                    Date = today, 
                    CompletedTaskIds = new List<int>()
                };

                _context.DailyTaskLogs.Add(log);
            }

            // Toggle logic
            if (request.IsCompleted)
            {
                if (!log.CompletedTaskIds.Contains(request.TaskId))
                    log.CompletedTaskIds.Add(request.TaskId);
            }
            else
            {
                log.CompletedTaskIds.Remove(request.TaskId);
            }

            await _context.SaveChangesAsync();

            return BaseResponse<bool>.Ok(true, "Task status updated.");
        }
    }
}
