using System.ComponentModel.DataAnnotations.Schema;


namespace Lumina.Models
{
    [Table(nameof(GoalProgress))]
    public class GoalProgress
    {
        public int Id { get; set; }

        public int GoalId { get; set; }
        public DateTime Date { get; set; }
        public int Value { get; set; }
        public string? Notes { get; set; }

        public Goal Goal { get; set; }
    }
}
