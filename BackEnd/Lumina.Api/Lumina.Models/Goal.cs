using System.ComponentModel.DataAnnotations.Schema;


namespace Lumina.Models
{
    [Table(nameof(Goal))]
    public class Goal
    {
        public int Id { get; set; }

        public required string Title { get; set; }
        public string? Description { get; set; }
        public GoalType Type { get; set; } // Habit, Achievement, Milestone
        public GoalFrequency Frequency { get; set; } // Daily, Weekly, Monthly
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int TargetValue { get; set; }
        public string Unit { get; set; } // times, minutes, pages, etc.
        public bool IsActive { get; set; }


        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<GoalProgress> Progress { get; set; } = new List<GoalProgress>();
    }
}
