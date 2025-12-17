using AutoMapper;
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
        private readonly IMapper _mapper;

        public ExerciseService(GGoneDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

            var responseList = _mapper.Map<List<ExerciseResponse>>(exercises);

            return new BaseResponse<List<ExerciseResponse>>(responseList);
        }

        public async Task<BaseResponse<ExerciseResponse>> GetExerciseById(int id)
        {
            var exercise = await _context.Exercises.FirstOrDefaultAsync(x => x.Id == id);

            var response = _mapper.Map<ExerciseResponse>(exercise);
           
            return new BaseResponse<ExerciseResponse>(response);
        }
    }
}
