using System.ComponentModel.DataAnnotations.Schema;


namespace Lumina.Models
{
    [Table(nameof(UserActivity))]
    public class UserActivity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Icon { get; set; }
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }


        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<JournalEntryActivity> JournalEntryActivities { get; set; } = new List<JournalEntryActivity>();

    }
}
