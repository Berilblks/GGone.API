using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGone.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIsManuallyUpdatedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquipmentApi",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "InstructionsApi",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "TargetApi",
                table: "Exercises");

            migrationBuilder.AddColumn<bool>(
                name: "IsManuallyUpdated",
                table: "Exercises",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsManuallyUpdated",
                table: "Exercises");

            migrationBuilder.AddColumn<string>(
                name: "EquipmentApi",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstructionsApi",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetApi",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
