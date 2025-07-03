using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Models
{
    [Table(nameof(UserJourney))]
    public class UserJourney
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ReflectionJourneyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int CurrentDay { get; set; }
        public bool IsActive { get; set; }

        public User User { get; set; }
        public ReflectionJourney Journey { get; set; }
    }
}
