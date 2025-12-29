using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GGone.API.Migrations
{
    /// <inheritdoc />
    public partial class TaskUpdateV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "DailyTaskLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "TaskItems",
                columns: new[] { "Id", "Category", "CreatedDate", "Description", "IsActive", "TaskId", "Title" },
                values: new object[,]
                {
                    { 1, "Nutrition", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Drink 3 liters of water per day.", true, "nut_water", "Drink Water" },
                    { 2, "Nutrition", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Follow your daily nutrition plan.", true, "nut_diet", "Stick to Your Diet" },
                    { 3, "Nutrition", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Consume only healthy snacks between meals.", true, "nut_snack", "Healthy Snacks" },
                    { 4, "Nutrition", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Do not consume fast food today.", true, "nut_fastfood", "Avoid Fast Food" },
                    { 5, "Nutrition", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Avoid carbonated and sugary drinks.", true, "nut_sugar", "No Sugary Drinks" },
                    { 6, "Physical Activity", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Walk 10,000 steps.", true, "phys_steps", "Step Goal" },
                    { 7, "Physical Activity", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Do 30 minutes of moderate cardio.", true, "phys_cardio", "Cardio Workout" },
                    { 8, "Physical Activity", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Do weight or bodyweight training.", true, "phys_strength", "Strength Training" },
                    { 9, "Sleep", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Get 7–8 hours of sleep.", true, "sleep_duration", "Sleep Duration" },
                    { 10, "Sleep", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Stop using your phone 1 hour before bed.", true, "sleep_detox", "Phone Detox" },
                    { 11, "Sleep", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Do 10 minutes of meditation.", true, "sleep_meditation", "Meditation" },
                    { 12, "Addiction", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "I did not smoke at all today.", true, "addict_smoking", "Did Not Smoke" },
                    { 13, "Addiction", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "I did not drink any alcohol today.", true, "addict_alcohol", "Did Not Drink Alcohol" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DailyTaskLogs");
        }
    }
}
