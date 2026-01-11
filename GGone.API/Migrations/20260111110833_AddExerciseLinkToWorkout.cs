using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGone.API.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseLinkToWorkout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExerciseId",
                table: "WorkoutExercises",
                type: "int",
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
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_Exercises_ExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutExercises_ExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.DropColumn(
                name: "ExerciseId",
                table: "WorkoutExercises");
        }
    }
}
