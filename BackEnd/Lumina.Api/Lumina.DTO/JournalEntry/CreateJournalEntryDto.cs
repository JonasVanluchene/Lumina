using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.DTO.JournalEntry
{
    public class CreateJournalEntryDto
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PrimaryMoodId { get; set; }
        public int MoodIntensity { get; set; }
        public string? Weather { get; set; }
        public int? SleepHours { get; set; }
    }
}
