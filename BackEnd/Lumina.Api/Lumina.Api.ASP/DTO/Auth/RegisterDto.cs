using System.ComponentModel.DataAnnotations;

namespace Lumina.Api.ASP.DTO.Auth
{
    public class RegisterDto
    {
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [EmailAddress]
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        public string? UserName { get; set; }
    }
}
