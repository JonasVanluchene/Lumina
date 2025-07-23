

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumina.Models
{
    [Table(nameof(RefreshToken))]
    public class RefreshToken
    {
        public int Id { get; set; }
        
        public required string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? ReplacedByToken { get; set; }
        public DateTime? RevokedAt { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }
        
        public required string UserId { get; set; }        
        public User User { get; set; } 
    }
}
