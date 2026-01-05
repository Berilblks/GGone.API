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

        public required string PlanName { get; set; } // Örn: "3 Day Split - Beginner"
        public string? Goal { get; set; } // Örn: "Hypertrophy", "Strength"
        public string? Difficulty { get; set; } // Örn: "Beginner"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public List<WorkoutDay> Days { get; set; } = new();
    }

    public class WorkoutDay
    {
        [Key]
        public int Id { get; set; }

        public int WorkoutPlanId { get; set; }
        public string DayName { get; set; } = "Day 1"; // "Monday", "Push Day", "Day 1"
        public string? FocusArea { get; set; } // "Chest & Triceps"

        // Navigation
        [JsonIgnore]
        public WorkoutPlan? WorkoutPlan { get; set; }
        public List<WorkoutExercise> Exercises { get; set; } = new();
    }

    public class WorkoutExercise
    {
        [Key]
        public int Id { get; set; }

        public int WorkoutDayId { get; set; }
        
        public int ExerciseId { get; set; } // Bizim DB'deki Exercise tablosuna FK
        public string? ExerciseNameSnapshot { get; set; } // Silinme ihtimaline karşı isim kopyası

        public string Sets { get; set; } = "3";
        public string Reps { get; set; } = "10";
        public string? Notes { get; set; } // "Drop set last one"

        // Navigation
        [JsonIgnore]
        public WorkoutDay? WorkoutDay { get; set; }

        [ForeignKey("ExerciseId")]
        public virtual GGone.API.Models.Exercises.Exercise? Exercise { get; set; }
    }
}
