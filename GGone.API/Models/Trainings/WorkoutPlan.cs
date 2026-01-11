using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GGone.API.Models.Trainings
{
    public class WorkoutPlan
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PlanName { get; set; }
        public string Goal { get; set; }
        public string Difficulty { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<WorkoutDay> Days { get; set; } = new();
    }

    public class WorkoutDay
    {
        [Key]
        public int Id { get; set; }
        public int WorkoutPlanId { get; set; }
        public string DayName { get; set; } // "Monday", etc.
        
        [JsonIgnore]
        public WorkoutPlan? WorkoutPlan { get; set; }
        public List<WorkoutExercise> Exercises { get; set; } = new();
    }

    public class WorkoutExercise
    {
        [Key]
        public int Id { get; set; }
        public int WorkoutDayId { get; set; }
        
        public string Name { get; set; }
        
        public int? ExerciseId { get; set; } // Nullable FK for image linking

        public int Sets { get; set; }
        public int Reps { get; set; }
        public string Notes { get; set; }

        [JsonIgnore]
        public WorkoutDay? WorkoutDay { get; set; }
        
        [ForeignKey("ExerciseId")]
        public virtual GGone.API.Models.Exercises.Exercise? Exercise { get; set; }
    }
}
