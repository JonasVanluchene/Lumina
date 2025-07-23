using System.ComponentModel.DataAnnotations.Schema;


namespace Lumina.Models
{
    [Table(nameof(UserTag))]
    public class UserTag
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }


        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<JournalEntryTag> JournalEntryTags { get; set; } = new List<JournalEntryTag>();
    }

}
