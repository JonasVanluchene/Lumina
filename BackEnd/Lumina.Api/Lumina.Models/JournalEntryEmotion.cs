using System.ComponentModel.DataAnnotations.Schema;


namespace Lumina.Models
{
    [Table(nameof(JournalEntryEmotion))]
    public class JournalEntryEmotion
    {

        public int Id { get; set; }

        public int JournalEntryId { get; set; }
        public int EmotionId { get; set; }

        public JournalEntry JournalEntry { get; set; }
        public Emotion Emotion { get; set; }
    }
}
