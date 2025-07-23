using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lumina.Repository;
using Microsoft.EntityFrameworkCore;

namespace Lumina.Services.BackgroundServices
{
    public class RefreshTokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RefreshTokenCleanupService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromDays(5); // Run every 5 days

        public RefreshTokenCleanupService(IServiceProvider serviceProvider, ILogger<RefreshTokenCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RefreshTokenCleanupService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupTokensAsync();
                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("RefreshTokenCleanupService stopped.");
        }

        private async Task CleanupTokensAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<LuminaDbContext>();

                var expirationThreshold = DateTime.UtcNow;

                var tokensToDelete = await dbContext.RefreshTokens
                    .Where(t =>
                        t.ExpiresAt < expirationThreshold ||
                        t.IsUsed && t.ExpiresAt.AddDays(1) < expirationThreshold ||
                        t.IsRevoked && t.ExpiresAt.AddDays(1) < expirationThreshold
                    )
                    .ToListAsync();

                if (tokensToDelete.Any())
                {
                    dbContext.RefreshTokens.RemoveRange(tokensToDelete);
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation("Deleted {Count} expired/used refresh tokens.", tokensToDelete.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while cleaning up refresh tokens.");
            }
        }
    }
}
