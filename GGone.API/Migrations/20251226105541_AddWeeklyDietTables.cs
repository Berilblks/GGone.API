using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGone.API.Migrations
{
    /// <inheritdoc />
    public partial class AddWeeklyDietTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeeklyDietPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyDietPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DietDay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Breakfast = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lunch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dinner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Snacks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WeeklyDietPlanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietDay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DietDay_WeeklyDietPlans_WeeklyDietPlanId",
                        column: x => x.WeeklyDietPlanId,
                        principalTable: "WeeklyDietPlans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DietDay_WeeklyDietPlanId",
                table: "DietDay",
                column: "WeeklyDietPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DietDay");

            migrationBuilder.DropTable(
                name: "WeeklyDietPlans");
        }
    }
}
