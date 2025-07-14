using System.ComponentModel.DataAnnotations.Schema;


namespace Lumina.Models
{
    [Table(nameof(Emotion))]
    public class Emotion
    {
        public int Id { get; set; }
        public string Name { get; set; } // Grateful, Excited, Lonely, Stressed
        public string Category { get; set; } // Positive, Negative, Neutral
        public string Icon { get; set; }
        public bool IsActive { get; set; }
    }
}
