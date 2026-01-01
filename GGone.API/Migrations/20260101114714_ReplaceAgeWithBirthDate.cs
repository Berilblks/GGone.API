using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGone.API.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceAgeWithBirthDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "BirthDate",
                table: "Users",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2000, 1, 1)); // Default value

            // Data Migration: Calculate estimated BirthDate from Age
            // We use SQL Server's DATEADD and GETDATE(). 
            // Since Age is just years, we subtract Age from current date.
            // Result will be today's date minus Age years.
            // CAST(GETDATE() AS DATE) ensures we work with date part only.
             migrationBuilder.Sql(
                @"UPDATE Users 
                  SET BirthDate = DATEADD(year, -Age, CAST(GETDATE() AS DATE))");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Reverse Data Migration: Calculate Age from BirthDate
            // DATEDIFF(year, BirthDate, GETDATE())
            migrationBuilder.Sql(
                @"UPDATE Users 
                  SET Age = DATEDIFF(year, BirthDate, GETDATE())");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Users");
        }
    }
}
