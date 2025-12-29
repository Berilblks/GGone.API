using GGone.API.Models.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GGone.API.Data.Configurations
{
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.HasData(
                // 1. Nutrition
                new TaskItem
                {
                    Id = 1,
                    TaskId = "nut_water",
                    Title = "Drink Water",
                    Description = "Drink 3 liters of water per day.",
                    Category = "Nutrition",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Sabit tarih eklendi
                },
                new TaskItem
                {
                    Id = 2,
                    TaskId = "nut_diet",
                    Title = "Stick to Your Diet",
                    Description = "Follow your daily nutrition plan.",
                    Category = "Nutrition",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Sabit tarih eklendi
                },
                new TaskItem
                {
                    Id = 3,
                    TaskId = "nut_snack",
                    Title = "Healthy Snacks",
                    Description = "Consume only healthy snacks between meals.",
                    Category = "Nutrition",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Sabit tarih eklendi
                },
                new TaskItem
                {
                    Id = 4,
                    TaskId = "nut_fastfood",
                    Title = "Avoid Fast Food",
                    Description = "Do not consume fast food today.",
                    Category = "Nutrition",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Sabit tarih eklendi
                },
                new TaskItem
                {
                    Id = 5,
                    TaskId = "nut_sugar",
                    Title = "No Sugary Drinks",
                    Description = "Avoid carbonated and sugary drinks.",
                    Category = "Nutrition",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Sabit tarih eklendi
                },

                // 2. Physical Activity
                new TaskItem
                {
                    Id = 6,
                    TaskId = "phys_steps",
                    Title = "Step Goal",
                    Description = "Walk 10,000 steps.",
                    Category = "Physical Activity",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Sabit tarih eklendi
                },
                new TaskItem
                {
                    Id = 7,
                    TaskId = "phys_cardio",
                    Title = "Cardio Workout",
                    Description = "Do 30 minutes of moderate cardio.",
                    Category = "Physical Activity",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Sabit tarih eklendi
                },
                new TaskItem
                {
                    Id = 8,
                    TaskId = "phys_strength",
                    Title = "Strength Training",
                    Description = "Do weight or bodyweight training.",
                    Category = "Physical Activity",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Sabit tarih eklendi
                },

                // 3. Sleep & Mental Health
                new TaskItem
                {
                    Id = 9,
                    TaskId = "sleep_duration",
                    Title = "Sleep Duration",
                    Description = "Get 7–8 hours of sleep.",
                    Category = "Sleep",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Sabit tarih eklendi
                },
                new TaskItem
                {
                    Id = 10,
                    TaskId = "sleep_detox",
                    Title = "Phone Detox",
                    Description = "Stop using your phone 1 hour before bed.",
                    Category = "Sleep",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Sabit tarih eklendi
                },
                new TaskItem
                {
                    Id = 11,
                    TaskId = "sleep_meditation",
                    Title = "Meditation",
                    Description = "Do 10 minutes of meditation.",
                    Category = "Sleep",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Sabit tarih eklendi
                },

                // 4. Addiction (General Templates)
                new TaskItem
                {
                    Id = 12,
                    TaskId = "addict_smoking",
                    Title = "Did Not Smoke",
                    Description = "I did not smoke at all today.",
                    Category = "Addiction",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Sabit tarih eklendi
                },
                new TaskItem
                {
                    Id = 13,
                    TaskId = "addict_alcohol",
                    Title = "Did Not Drink Alcohol",
                    Description = "I did not drink any alcohol today.",
                    Category = "Addiction",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Sabit tarih eklendi
                }
            );
        }
    }
}
