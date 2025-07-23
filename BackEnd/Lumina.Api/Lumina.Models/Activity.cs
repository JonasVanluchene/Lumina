using System.ComponentModel.DataAnnotations.Schema;


namespace Lumina.Models
{
    [Table(nameof(Activity))]
    public class Activity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Icon { get; set; }
        public string Category { get; set; } // Work, Health, Social, Personal
        public bool IsSystemDefined { get; set; }
        public int SortOrder { get; set; }

        public ICollection<JournalEntryActivity> JournalEntryActivities { get; set; } = new List<JournalEntryActivity>();
    }
}
