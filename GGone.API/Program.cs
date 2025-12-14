using GGone.API.Business.Abstracts;
using GGone.API.Business.Services.Auth;
using GGone.API.Business.Services.Exercises;
using GGone.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
