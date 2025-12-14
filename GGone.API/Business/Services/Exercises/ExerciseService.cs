using GGone.API.Business.Abstracts;
using GGone.API.Data;
using GGone.API.Models;
using GGone.API.Models.Enum;
using GGone.API.Models.Exercises;
using Microsoft.EntityFrameworkCore;

namespace GGone.API.Business.Services.Exercises
{
    public class ExerciseService : IExerciseService
    {
        private readonly GGoneDbContext _context;

        public ExerciseService(GGoneDbContext context)
        {
            _context = context;
        }
        public async Task<BaseResponse<List<ExerciseResponse>>> GetExercises(ExerciseFilterRequest request)
        {
            var query = _context.Exercises.AsQueryable();

            if (request.BodyPart.HasValue)
                query = query.Where(x => x.BodyPart == (BodyPart)request.BodyPart.Value);

            if (request.ExerciseLevel.HasValue)
                query = query.Where(x => x.ExerciseLevel == (ExerciseLevel)request.ExerciseLevel.Value);

            if (request.IsHome.HasValue)
                query = query.Where(x => x.IsHome == request.IsHome.Value);

            var exercises = await query.ToListAsync();

            var responseList = exercises.Select(x => new ExerciseResponse
            {
                Id = x.Id,
                Name = x.Name,
                ImageUrl = x.ImageUrl,
                Description = x.Description,
                Detail = x.Detail,
                BodyPart = x.BodyPart,
                ExerciseLevel = x.ExerciseLevel,
                IsHome = x.IsHome
            }).ToList();

            return new BaseResponse<List<ExerciseResponse>>(responseList);
        }

        public async Task<BaseResponse<ExerciseResponse>> GetExerciseById(int id)
        {
            var exercise = await _context.Exercises.FirstOrDefaultAsync(x => x.Id == id);

            var response = new ExerciseResponse
            {
                Id = exercise.Id,
                Name = exercise.Name,
                ImageUrl = exercise.ImageUrl,
                Description = exercise.Description,
                Detail = exercise.Detail,
                BodyPart = exercise.BodyPart,
                ExerciseLevel = exercise.ExerciseLevel,
                IsHome = exercise.IsHome
            };

            return new BaseResponse<ExerciseResponse>(response);
        }
    }
}
