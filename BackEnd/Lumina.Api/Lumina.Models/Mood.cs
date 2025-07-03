using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Models
{
    [Table(nameof(Mood))]
    public class Mood
    {
        public int Id { get; set; }
        public string Name { get; set; } // Happy, Sad, Angry, Calm, Anxious
        public string Icon { get; set; } // Unicode emoji or icon class
        public string Color { get; set; } // Hex color code
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
