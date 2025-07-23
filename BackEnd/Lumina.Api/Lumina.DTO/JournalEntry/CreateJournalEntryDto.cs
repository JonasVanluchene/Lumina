

namespace Lumina.DTO.JournalEntry
{
    public class CreateJournalEntryDto
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public DateTime Date { get; set; }
        public int PrimaryMoodId { get; set; }
        public int MoodIntensity { get; set; }
        public string? Weather { get; set; }
        public int? SleepHours { get; set; }

        public List<string> TagNames { get; set; } = new();
        public List<string> ActivityNames { get; set; } = new();

        //public List<int> TagIds { get; set; } = new();
        //public List<int> UserTagIds { get; set; } = new();
        //public List<string> NewUserTagNames { get; set; } = new();

        //public List<int> ActivityIds { get; set; } = new();
        //public List<int> UserActivityIds { get; set; } = new();
        //public List<string> NewUserActivityNames { get; set; } = new();

        public List<int> SecondaryEmotionIds { get; set; } = new();
    }
}
