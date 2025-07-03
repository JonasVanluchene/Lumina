using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Models
{
    [Table(nameof(JournalEntryActivity))]
    public class JournalEntryActivity
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        public int? ActivityId { get; set; }
        public int? UserActivityId { get; set; }

        public JournalEntry JournalEntry { get; set; }
        public Activity? Activity { get; set; }
        public UserActivity? UserActivity { get; set; }
    }
}
