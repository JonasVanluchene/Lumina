namespace Lumina.Api.ASP.DTO
{
    public class ErrorResponse
    {
        public required string Message { get; set; }
        public string? Details { get; set; }
        public string? Code { get; set; }

       
    }
}
