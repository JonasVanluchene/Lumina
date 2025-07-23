using System.ComponentModel.DataAnnotations;


namespace Lumina.DTO.Activity
{
    public class CreateUserActivityDto
    {
        [Required(ErrorMessage = "Activity name is required")]
        [MinLength(2, ErrorMessage = "Activity name must be at least 2 characters long")]
        [MaxLength(30, ErrorMessage = "Activity name can't be longer than 30 characters")]
        public required string Name { get; set; }

        public string? Category { get; set; }
    }
}
