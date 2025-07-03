using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Models
{
    [Table(nameof(Activity))]
    public class Activity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Icon { get; set; }
        public string Category { get; set; } // Work, Health, Social, Personal
        public bool IsSystemDefined { get; set; }
        public int SortOrder { get; set; }
    }
}
