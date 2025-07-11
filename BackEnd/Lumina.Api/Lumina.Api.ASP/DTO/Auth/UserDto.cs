namespace Lumina.Api.ASP.DTO.Auth
{
    public class UserDto
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public string? UserName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Token { get; set; }
    }
}
