using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGone.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWorkoutPlanStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clear existing data to avoid conversion errors
            migrationBuilder.Sql("DELETE FROM WorkoutExercises");
            migrationBuilder.Sql("DELETE FROM WorkoutDays");
            migrationBuilder.Sql("DELETE FROM WorkoutPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_Exercises_ExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutExercises_ExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.DropColumn(
                name: "ExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.DropColumn(
                name: "ExerciseNameSnapshot",
                table: "WorkoutExercises");

            migrationBuilder.DropColumn(
                name: "FocusArea",
                table: "WorkoutDays");

            migrationBuilder.AlterColumn<string>(
                name: "Goal",
                table: "WorkoutPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Difficulty",
                table: "WorkoutPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "WorkoutPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "Sets",
                table: "WorkoutExercises",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Reps",
                table: "WorkoutExercises",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "WorkoutExercises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "WorkoutExercises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "WorkoutPlans");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "WorkoutExercises");

            migrationBuilder.AlterColumn<string>(
                name: "Goal",
                table: "WorkoutPlans",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Difficulty",
                table: "WorkoutPlans",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Sets",
                table: "WorkoutExercises",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Reps",
                table: "WorkoutExercises",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "WorkoutExercises",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ExerciseId",
                table: "WorkoutExercises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ExerciseNameSnapshot",
                table: "WorkoutExercises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FocusArea",
                table: "WorkoutDays",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_ExerciseId",
                table: "WorkoutExercises",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_Exercises_ExerciseId",
                table: "WorkoutExercises",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
