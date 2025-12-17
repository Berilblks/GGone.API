using AutoMapper;
using GGone.API.Business.Abstracts;
using GGone.API.Data;
using GGone.API.Models;
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

        public async Task<BaseResponse<List<DailyTaskResponse>>> GetTodayTasks()
        {
            // 1. Aktif olan tüm görev tanımlarını çek
            var tasks = await _context.TaskItems.Where(x => x.IsActive).ToListAsync();

            // 2. Bugünün tamamlama logunu bul
            var today = DateTime.UtcNow.Date;
            var log = await _context.DailyTaskLogs.FirstOrDefaultAsync(x => x.Date.Date == today);

            // 3. Mapper ile dönüşümü yap
            var response = _mapper.Map<List<DailyTaskResponse>>(tasks);

            // 4. Tamamlanma durumlarını log'a göre işaretle
            if (log != null)
            {
                foreach (var item in response)
                {
                    item.IsCompleted = log.CompletedTaskIds.Contains(item.Id);
                }
            }

            return BaseResponse<List<DailyTaskResponse>>.Ok(response);
        }

        public async Task<BaseResponse<bool>> ToggleTaskCompletion(ToggleCompletionRequest request)
        {
            var today = DateTime.UtcNow.Date;
            var log = await _context.DailyTaskLogs.FirstOrDefaultAsync(x => x.Date.Date == today);

            // Eğer bugün için log kaydı yoksa yeni oluştur
            if (log == null)
            {
                log = new DailyTaskLog { Date = today, CompletedTaskIds = new List<string>() };
                _context.DailyTaskLogs.Add(log);
            }

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
