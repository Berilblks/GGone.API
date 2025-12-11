using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGone.API.Migrations
{
    /// <inheritdoc />
    public partial class AddConsumptionAndPriceToAddiction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DidConsume",
                table: "Addictions");

            migrationBuilder.RenameColumn(
                name: "RecordDate",
                table: "Addictions",
                newName: "LastConsumptionDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPasswordExpires",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetPasswordToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DailyConsumption",
                table: "Addictions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "UnitPrice",
                table: "Addictions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetPasswordExpires",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetPasswordToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DailyConsumption",
                table: "Addictions");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "Addictions");

            migrationBuilder.RenameColumn(
                name: "LastConsumptionDate",
                table: "Addictions",
                newName: "RecordDate");

            migrationBuilder.AddColumn<bool>(
                name: "DidConsume",
                table: "Addictions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
