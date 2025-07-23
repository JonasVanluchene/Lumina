using Lumina.DTO.JournalEntry;
using Lumina.Models;
using System.Linq;


namespace Lumina.Services.Helpers
{
    public static class JournalEntryProjectionHelper
    {
        public static IQueryable<JournalEntryDto?> ProjectToDto(IQueryable<JournalEntry> query)
        {
            return query.Select(j => new JournalEntryDto
            {
                Id = j.Id,
                Title = j.Title,
                Content = j.Content ?? string.Empty,
                Date = j.Date,
                CreatedAt = j.CreatedAt,
                UpdatedAt = j.UpdatedAt,
                PrimaryMoodId = j.PrimaryMoodId,
                PrimaryMoodName = j.PrimaryMood != null ? j.PrimaryMood.Name : null,
                MoodIntensity = j.MoodIntensity,
                Weather = j.Weather,
                SleepHours = j.SleepHours,
                Tags = j.Tags.Where(t => t.Tag != null).Select(t => t.Tag.Name).ToList(),
                UserTags = j.Tags.Where(t => t.UserTag != null).Select(t => t.UserTag.Name).ToList(),
                Activities = j.Activities.Where(a => a.Activity != null).Select(a => a.Activity.Name).ToList(),
                UserActivities = j.Activities.Where(a => a.UserActivity != null).Select(a => a.UserActivity.Name).ToList(),
                SecondaryEmotions = j.SecondaryEmotions.Select(se => se.Emotion.Name).ToList(),
                UserId = j.UserId
            });
        }
    }
}
