using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGone.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendshipTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietDay_WeeklyDietPlans_WeeklyDietPlanId",
                table: "DietDay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DietDay",
                table: "DietDay");

            migrationBuilder.RenameTable(
                name: "DietDay",
                newName: "DietDays");

            migrationBuilder.RenameIndex(
                name: "IX_DietDay_WeeklyDietPlanId",
                table: "DietDays",
                newName: "IX_DietDays_WeeklyDietPlanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DietDays",
                table: "DietDays",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Friendships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FriendId = table.Column<int>(type: "int", nullable: false),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Friendships_Users_FriendId",
                        column: x => x.FriendId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friendships_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_FriendId",
                table: "Friendships",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId",
                table: "Friendships",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DietDays_WeeklyDietPlans_WeeklyDietPlanId",
                table: "DietDays",
                column: "WeeklyDietPlanId",
                principalTable: "WeeklyDietPlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietDays_WeeklyDietPlans_WeeklyDietPlanId",
                table: "DietDays");

            migrationBuilder.DropTable(
                name: "Friendships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DietDays",
                table: "DietDays");

            migrationBuilder.RenameTable(
                name: "DietDays",
                newName: "DietDay");

            migrationBuilder.RenameIndex(
                name: "IX_DietDays_WeeklyDietPlanId",
                table: "DietDay",
                newName: "IX_DietDay_WeeklyDietPlanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DietDay",
                table: "DietDay",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DietDay_WeeklyDietPlans_WeeklyDietPlanId",
                table: "DietDay",
                column: "WeeklyDietPlanId",
                principalTable: "WeeklyDietPlans",
                principalColumn: "Id");
        }
    }
}
