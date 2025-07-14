

namespace Lumina.DTO.JournalEntry
{
    public class JournalEntryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public DateTime Date { get; set; }
        public int MoodIntensity { get; set; }
        public string PrimaryMoodName { get; set; }
        public string? Weather { get; set; }

        public int SleepHours { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
        public List<string> SecondaryEmotions { get; set; } = new List<string>();
    }
}
