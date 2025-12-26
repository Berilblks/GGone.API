using GGone.API.Business.Abstracts;
using GGone.API.Business.Services.Addiction;
using GGone.API.Business.Services.AI;
using GGone.API.Business.Services.Auth;
using GGone.API.Business.Services.BMI;
using GGone.API.Business.Services.Exercises;
using GGone.API.Business.Services.Tasks;
using GGone.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.AddConsole();


// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<GGoneDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


//Login and Token Services
builder.Services.AddScoped<IAuthService, AuthService>();

// Exercise Services
builder.Services.AddScoped<IExerciseDataFetcher, RapidExerciseDataFetcher>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();

builder.Services.AddScoped<IAddictionService, AddictionService>();
builder.Services.AddScoped<IBmiService, BMIService>();

builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddHttpClient<IAIChatService, GeminiChatService>();

builder.Services.AddControllers().AddJsonOptions(x =>
   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
