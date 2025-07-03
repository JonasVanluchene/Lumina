using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Models
{
    [Table(nameof(JourneyPrompt))]
    public class JourneyPrompt
    {
        public int Id { get; set; }
        public int ReflectionJourneyId { get; set; }
        public int DayNumber { get; set; }
        public string PromptText { get; set; }
        public string? AdditionalInstructions { get; set; }

        public ReflectionJourney Journey { get; set; }
    }
}
