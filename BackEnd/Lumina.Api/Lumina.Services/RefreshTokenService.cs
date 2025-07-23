using Lumina.Models;
using Lumina.Repository;
using Lumina.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Lumina.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly LuminaDbContext _dbContext;
        private readonly JwtTokenService _jwtTokenService;

        public RefreshTokenService(LuminaDbContext dbContext, JwtTokenService jwtTokenService)
        {
            _dbContext = dbContext;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<RefreshToken> CreateRefreshTokenAsync(User user)
        {
            var refreshToken = new RefreshToken
            {
                Token = HashToken(_jwtTokenService.GenerateRefreshToken()),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id,
                IsRevoked = false,
                IsUsed = false
            };
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<RefreshToken?> GetValidRefreshTokenAsync(string token)
        {
            var hashedToken = HashToken(token);
            return await _dbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == hashedToken && !rt.IsUsed && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);
        }

        public async Task InvalidateRefreshTokenAsync(RefreshToken token)
        {
            token.IsUsed = true;
            token.RevokedAt = DateTime.UtcNow;
            _dbContext.RefreshTokens.Update(token);
            await _dbContext.SaveChangesAsync();
        }

        private static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }
    }
}
