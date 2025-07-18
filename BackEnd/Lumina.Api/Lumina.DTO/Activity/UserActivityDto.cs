using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.DTO.Activity
{
    public class UserActivityDto
    {
        public string Name { get; set; }
        public string Category { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
