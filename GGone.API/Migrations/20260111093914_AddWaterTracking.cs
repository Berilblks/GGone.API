using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGone.API.Migrations
{
    /// <inheritdoc />
    public partial class AddWaterTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentWaterIntake",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastWaterDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentWaterIntake",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastWaterDate",
                table: "Users");
        }
    }
}
