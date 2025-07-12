using System.ComponentModel.DataAnnotations.Schema;


namespace Lumina.Models
{
    [Table(nameof(Tag))]
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<JournalEntryTag> JournalEntryTags { get; set; } = new List<JournalEntryTag>();
    }
}
