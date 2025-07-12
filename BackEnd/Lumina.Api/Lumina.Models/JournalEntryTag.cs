using System.ComponentModel.DataAnnotations.Schema;


namespace Lumina.Models
{
    [Table(nameof(JournalEntryTag))]
    public class JournalEntryTag
    {
        public int Id { get; set; }

        public int JournalEntryId { get; set; }
        public JournalEntry JournalEntry { get; set; }

        public int? TagId { get; set; }
        public Tag? Tag { get; set; }
        public int? UserTagId { get; set; }
        public UserTag? UserTag { get; set; }
    }
}
