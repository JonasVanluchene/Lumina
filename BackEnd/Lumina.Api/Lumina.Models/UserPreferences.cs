using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Models
{
    [Table(nameof(UserPreferences))]
    public class UserPreferences
    {
        public int Id { get; set; }
        
        public string Theme { get; set; } = "light"; // light, dark, auto
        public bool EmailNotifications { get; set; } = true;
        public bool DailyReminders { get; set; } = false;
        public TimeSpan? ReminderTime { get; set; }
        public string Timezone { get; set; }
        public string Language { get; set; } = "nl-BE";

        public required string UserId { get; set; }
        public User User { get; set; }
    }
}
