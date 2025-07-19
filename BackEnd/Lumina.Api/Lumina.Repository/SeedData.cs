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

            // Seed Moods
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
            }

            // Seed Tags
            if (!context.Tags.Any())
            {
                var tags = new List<Tag>
                {
                    new Tag { Name = "Work", IsSystemDefined = true },
                    new Tag { Name = "Family", IsSystemDefined = true },
                    new Tag { Name = "Health" , IsSystemDefined = true},
                    new Tag { Name = "Study" , IsSystemDefined = true},
                    new Tag { Name = "Social", IsSystemDefined = true}
                };

                context.Tags.AddRange(tags);
            }

            // Seed Emotions
            if (!context.Emotions.Any())
            {
                var emotions = new List<Emotion>
                {
                    new Emotion { Name = "Grateful", Category = "Positive", Icon = "🙏", IsActive = true },
                    new Emotion { Name = "Excited", Category = "Positive", Icon = "🎉", IsActive = true },
                    new Emotion { Name = "Lonely", Category = "Negative", Icon = "😞", IsActive = true },
                    new Emotion { Name = "Stressed", Category = "Negative", Icon = "😫", IsActive = true },
                    new Emotion { Name = "Content", Category = "Neutral", Icon = "😐", IsActive = true }
                };

                context.Emotions.AddRange(emotions);
            }
            //  Seed Activities
            if (!context.Activities.Any())
            {
                var activities = new List<Activity>
                {
                    new Activity { Name = "Exercise", Icon = "🏋️‍♂️", Category = "Health", IsSystemDefined = true, SortOrder = 1 },
                    new Activity { Name = "Work", Icon = "💻", Category = "Work", IsSystemDefined = true, SortOrder = 2 },
                    new Activity { Name = "Reading", Icon = "📚", Category = "Personal", IsSystemDefined = true, SortOrder = 3 },
                    new Activity { Name = "Socializing", Icon = "🎉", Category = "Social", IsSystemDefined = true, SortOrder = 4 },
                    new Activity { Name = "Meditation", Icon = "🧘‍♀️", Category = "Health", IsSystemDefined = true, SortOrder = 5 }
                };

                context.Activities.AddRange(activities);
            }
            await context.SaveChangesAsync();
        }

    }
}
