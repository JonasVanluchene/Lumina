using System.ComponentModel.DataAnnotations.Schema;


namespace Lumina.Models
{
    [Table(nameof(ReflectionJourney))]
    public class ReflectionJourney
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationDays { get; set; }
        public string Category { get; set; } // Gratitude, Mindfulness, Self-Care
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<JourneyPrompt> Prompts { get; set; } = new List<JourneyPrompt>();
        public ICollection<UserJourney> UserJourneys { get; set; } = new List<UserJourney>();
    }
}
