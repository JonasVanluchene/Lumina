using System.ComponentModel.DataAnnotations.Schema;


namespace Lumina.Models
{
    [Table(nameof(JournalEntry))]
    public class JournalEntry
    {
        public int Id { get; set; }

        public required string Title { get; set; }
        public string? Content { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Mood Data
        public int PrimaryMoodId { get; set; }
        public int MoodIntensity { get; set; } // 1-5 scale

        // Weather (optional context)
        public string? Weather { get; set; }
        public int? SleepHours { get; set; }

        // Foreign Keys
        public string UserId { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Mood PrimaryMood { get; set; }
        public ICollection<JournalEntryEmotion> SecondaryEmotions { get; set; } = new List<JournalEntryEmotion>();
        public ICollection<JournalEntryActivity> Activities { get; set; } = new List<JournalEntryActivity>();
        public ICollection<JournalEntryTag> Tags { get; set; } = new List<JournalEntryTag>();
        public ICollection<JournalEntryAttachment> Attachments { get; set; } = new List<JournalEntryAttachment>();
    }
}
