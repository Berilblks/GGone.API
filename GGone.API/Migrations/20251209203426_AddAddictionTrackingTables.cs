using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGone.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAddictionTrackingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRelapseDate",
                table: "Addictions");

            migrationBuilder.DropColumn(
                name: "MilestonesDays",
                table: "Addictions");

            migrationBuilder.AddColumn<int>(
                name: "ActiveDays",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "LastLoginDate",
                table: "Users",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AlterColumn<int>(
                name: "AddictionType",
                table: "Addictions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DidConsume",
                table: "Addictions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecordDate",
                table: "Addictions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveDays",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DidConsume",
                table: "Addictions");

            migrationBuilder.DropColumn(
                name: "RecordDate",
                table: "Addictions");

            migrationBuilder.AlterColumn<string>(
                name: "AddictionType",
                table: "Addictions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRelapseDate",
                table: "Addictions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MilestonesDays",
                table: "Addictions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
