using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.DTO.Tag
{
    public class UpdateUserTagDto
    {
        [Required(ErrorMessage = "Tag name is required")]
        [MinLength(2, ErrorMessage = "Tag must be at least 2 characters long")]
        [MaxLength(30, ErrorMessage = "Tag can't be longer than 30 characters")]
        public required string Name { get; set; }
    }
}
