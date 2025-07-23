using Lumina.Models;

namespace Lumina.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> CreateRefreshTokenAsync(User user);
        Task<RefreshToken?> GetValidRefreshTokenAsync(string token);
        Task InvalidateRefreshTokenAsync(RefreshToken token);
    }
}
