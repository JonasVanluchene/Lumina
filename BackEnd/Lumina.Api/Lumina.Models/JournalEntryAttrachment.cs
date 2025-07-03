using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Models
{
    [Table(nameof(JournalEntryAttachment))]
    public class JournalEntryAttachment
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        public JournalEntry JournalEntry { get; set; }

        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string? Description { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
