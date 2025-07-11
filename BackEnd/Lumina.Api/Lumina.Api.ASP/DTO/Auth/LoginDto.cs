namespace Lumina.Api.ASP.DTO.Auth
{
    public class LoginDto
    {
        public required string Identifier { get; set; }
        public required string Password { get; set; }
    }
}
