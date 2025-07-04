using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumina.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Lumina.Repository
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LuminaDbContext>();

            if (!context.Moods.Any())
            {
                var moods = new List<Mood>
                {
                    new Mood { Name = "Happy", Icon = "😊", Color = "#FFD700", SortOrder = 1, IsActive = true },
                    new Mood { Name = "Sad", Icon = "😢", Color = "#1E90FF", SortOrder = 2, IsActive = true },
                    new Mood { Name = "Angry", Icon = "😠", Color = "#FF4500", SortOrder = 3, IsActive = true },
                    new Mood { Name = "Calm", Icon = "😌", Color = "#32CD32", SortOrder = 4, IsActive = true },
                    new Mood { Name = "Anxious", Icon = "😰", Color = "#8A2BE2", SortOrder = 5, IsActive = true }
                };

                context.Moods.AddRange(moods);
                await context.SaveChangesAsync();
            }
        }
    }
}
