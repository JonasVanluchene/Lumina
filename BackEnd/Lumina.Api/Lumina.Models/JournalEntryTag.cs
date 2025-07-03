using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Models
{
    [Table(nameof(JournalEntryTag))]
    public class JournalEntryTag
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        public JournalEntry JournalEntry { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
