using Lumina.DTO.Activity;
using Lumina.DTO.Tag;
using Lumina.Models;
using Lumina.Repository;
using Lumina.Services;
using Lumina.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lumina.Services.Helpers
{
    public static class JournalEntryHelper
    {
        public static async Task AddTagsToEntryAsync(ITagService tagService,string userId,LuminaDbContext dbContext,IEnumerable<string> tagNames,JournalEntry journalEntry)
        {
            //Check and add tags
            foreach (var tagName in tagNames)
            {
                var trimmedName = tagName.Trim();

                // 1. Check for system tag
                var systemTag = await dbContext.Tags
                    .FirstOrDefaultAsync(t => t.IsSystemDefined && t.Name == trimmedName);
                if (systemTag != null)
                {
                    journalEntry.Tags.Add(new JournalEntryTag
                    {
                        Tag = systemTag
                    });
                    continue;
                }

                // 2. Check for user tag
                var userTag = await dbContext.UserTags
                    .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.Name == trimmedName && ut.IsActive);
                if (userTag != null)
                {
                    journalEntry.Tags.Add(new JournalEntryTag { UserTag = userTag });
                    continue;
                }

                // 3. Create new user tag
                var createUserTagDto = new CreateUserTagDto { Name = trimmedName };
                var newTagDto = await tagService.CreateUserTagAsync(createUserTagDto, userId);
                var newUserTag = await dbContext.UserTags.FirstAsync(ut => ut.Id == newTagDto.Id);
                journalEntry.Tags.Add(new JournalEntryTag { UserTag = newUserTag });
            }

        }

        public static async Task AddActivitiessToEntryAsync(IActivityService activityService, string userId, LuminaDbContext dbContext, IEnumerable<string> activityNames, JournalEntry journalEntry)
        {
            //Check and add activities
            foreach (var activityName in activityNames)
            {
                var trimmedName = activityName.Trim();

                // 1. Check for system activity
                var systemActivity = await dbContext.Activities
                    .FirstOrDefaultAsync(a => a.IsSystemDefined && a.Name == trimmedName);
                if (systemActivity != null)
                {
                    journalEntry.Activities.Add(new JournalEntryActivity
                    {
                        Activity = systemActivity
                    });
                    continue;
                }

                // 2. Check for user activity
                var userActivity = await dbContext.UserActivities
                    .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.Name == trimmedName && ua.IsActive);
                if (userActivity != null)
                {
                    journalEntry.Activities.Add(new JournalEntryActivity { UserActivity = userActivity });
                    continue;
                }

                // 3. Create new user activity
                var createUserActivityDto = new CreateUserActivityDto { Name = trimmedName };
                var newActivityDto = await activityService.CreateUserActivityAsync(createUserActivityDto, userId);
                var newUserActivity = await dbContext.UserActivities.FirstAsync(ua => ua.Id == newActivityDto.Id);
                journalEntry.Activities.Add(new JournalEntryActivity { UserActivity = newUserActivity });
            }

        }
    }
}
