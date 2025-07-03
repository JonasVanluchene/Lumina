using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Models
{
    [Table(nameof(User))]
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        
        public UserPreferences Preferences { get; set; }

        // Navigation Properties
        public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
        public ICollection<UserActivity> CustomActivities { get; set; } = new List<UserActivity>();
        public ICollection<UserJourney> ActiveJourneys { get; set; } = new List<UserJourney>();
        public ICollection<Goal> Goals { get; set; } = new List<Goal>();
    }
}
