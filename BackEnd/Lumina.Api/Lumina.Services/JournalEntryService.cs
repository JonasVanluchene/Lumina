using AutoMapper;
using Lumina.DTO.JournalEntry;
using Lumina.DTO.Tag;
using Lumina.DTO.Activity;
using Lumina.Repository;
using Lumina.Services.Interfaces;
using Lumina.Models;
using Microsoft.EntityFrameworkCore;
using Lumina.Services.Helpers;

namespace Lumina.Services
{
    public class JournalEntryService : IJournalEntryService
    {
        private readonly LuminaDbContext _dbContext;
        private readonly ITagService _tagService;
        private readonly IActivityService _activityService;

        private readonly IMapper _mapper;

        public JournalEntryService(LuminaDbContext dbContext, IMapper mapper, ITagService tagService, IActivityService activityService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _tagService = tagService;
            _activityService = activityService;
        }

        public async Task<IEnumerable<JournalEntryDto>> GetAllAsync(string userId)
        {
            var journalEntries = await JournalEntryHelper
            .ProjectToDto(_dbContext.JournalEntries.Where(j => j.UserId == userId).OrderByDescending(j => j.CreatedAt))
            .ToListAsync();

            return journalEntries;
           
        }

        public async Task<JournalEntryDto?> GetByIdAsync(int id, string userId)
        {
            var journalEntry = await JournalEntryHelper
            .ProjectToDto(_dbContext.JournalEntries.Where(j => j.UserId == userId && j.Id == id))
            .FirstOrDefaultAsync();

            return journalEntry;

        }

        public async Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto dto, string userId)
        {
            var journalEntry = _mapper.Map<JournalEntry>(dto);
            journalEntry.UserId = userId;
            journalEntry.CreatedAt = DateTime.UtcNow;


            //Check and add tags
            foreach (var tagName in dto.TagNames) // Assume TagNames is a List<string>
            {
                var trimmedName = tagName.Trim();

                // 1. Check for system tag
                var systemTag = await _dbContext.Tags
                    .FirstOrDefaultAsync(t => t.IsSystemDefined && t.Name == trimmedName);
                if (systemTag != null)
                {
                    journalEntry.Tags.Add(new JournalEntryTag { Tag = systemTag });
                    continue;
                }

                // 2. Check for user tag
                var userTag = await _dbContext.UserTags
                    .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.Name == trimmedName && ut.IsActive);
                if (userTag != null)
                {
                    journalEntry.Tags.Add(new JournalEntryTag { UserTag = userTag });
                    continue;
                }

                // 3. Create new user tag
                var createUserTagDto = new CreateUserTagDto { Name = trimmedName };
                var newTagDto = await _tagService.CreateUserTagAsync(createUserTagDto, userId);
                var newUserTag = await _dbContext.UserTags.FirstAsync(ut => ut.Id == newTagDto.Id);
                journalEntry.Tags.Add(new JournalEntryTag { UserTag = newUserTag });
            }

            //Check and add activities
            foreach (var activityName in dto.ActivityNames)
            {
                var trimmedName = activityName.Trim();

                // 1. Check for system activity
                var systemActivity = await _dbContext.Activities
                    .FirstOrDefaultAsync(t => t.IsSystemDefined && t.Name == trimmedName);
                if (systemActivity != null)
                {
                    journalEntry.Activities.Add(new JournalEntryActivity { Activity = systemActivity });
                    continue;
                }

                // 2. Check for user activity
                var userActivity = await _dbContext.UserActivities
                    .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.Name == trimmedName && ua.IsActive);
                if (userActivity != null)
                {
                    journalEntry.Activities.Add(new JournalEntryActivity { UserActivity = userActivity });
                    continue;
                }

                // 3. Create new user activity
                var createUserActivityDto = new CreateUserActivityDto { Name = trimmedName };
                var newActivityDto = await _activityService.CreateUserActivityAsync(createUserActivityDto, userId);
                var newUserActivity = await _dbContext.UserActivities.FirstAsync(ut => ut.Id == newActivityDto.Id);
                journalEntry.Activities.Add(new JournalEntryActivity { UserActivity = newUserActivity });
            }


            // Validate and attach existing Emotions
            var emotions = await _dbContext.Emotions
            .Where(e => dto.SecondaryEmotionIds.Contains(e.Id))
            .ToListAsync();

            //if (emotions.Count != dto.SecondaryEmotionIds.Count)
            //    throw new Exception("One or more EmotionIds are invalid.");

            foreach (var emotion in emotions)
            {
                journalEntry.SecondaryEmotions.Add(new JournalEntryEmotion { Emotion = emotion });
            }

            await _dbContext.JournalEntries.AddAsync(journalEntry);
            await _dbContext.SaveChangesAsync();

            // Project to DTO instead of loading with .Include
            var savedEntryDto = await JournalEntryHelper
            .ProjectToDto(_dbContext.JournalEntries.Where(j => j.UserId == userId && j.Id == journalEntry.Id))
            .FirstOrDefaultAsync();            

            return savedEntryDto;
        }

        public async Task<JournalEntryDto?> UpdateAsync(int id, UpdateJournalEntryDto dto, string userId)
        {
            var existingEntry = await _dbContext.JournalEntries
                .Include(j => j.Tags)
                .Include(j => j.Activities)
                .Include(j => j.SecondaryEmotions)
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

            if (existingEntry == null)
                return null;

            _mapper.Map(dto, existingEntry);
            existingEntry.UpdatedAt = DateTime.UtcNow;

            // Update Tags
            existingEntry.Tags.Clear();
            foreach (var tagName in dto.TagNames)
            {
                var trimmedName = tagName.Trim();

                // 1. Check for system tag
                var systemTag = await _dbContext.Tags
                    .FirstOrDefaultAsync(t => t.IsSystemDefined && t.Name == trimmedName);
                if (systemTag != null)
                {
                    existingEntry.Tags.Add(new JournalEntryTag { Tag = systemTag });
                    continue;
                }

                // 2. Check for user tag
                var userTag = await _dbContext.UserTags
                    .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.Name == trimmedName && ut.IsActive);
                if (userTag != null)
                {
                    existingEntry.Tags.Add(new JournalEntryTag { UserTag = userTag });
                    continue;
                }

                // 3. Create new user tag
                var createUserTagDto = new CreateUserTagDto { Name = trimmedName };
                var newTagDto = await _tagService.CreateUserTagAsync(createUserTagDto, userId);
                var newUserTag = await _dbContext.UserTags.FirstAsync(ut => ut.Id == newTagDto.Id);
                existingEntry.Tags.Add(new JournalEntryTag { UserTag = newUserTag });
            }

            // Update Activities
            existingEntry.Activities.Clear();
            foreach (var activityName in dto.ActivityNames)
            {
                var trimmedName = activityName.Trim();

                // 1. Check for system activity
                var systemActivity = await _dbContext.Activities
                    .FirstOrDefaultAsync(t => t.IsSystemDefined && t.Name == trimmedName);
                if (systemActivity != null)
                {
                    existingEntry.Activities.Add(new JournalEntryActivity { Activity = systemActivity });
                    continue;
                }

                // 2. Check for user activity
                var userActivity = await _dbContext.UserActivities
                    .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.Name == trimmedName && ua.IsActive);
                if (userActivity != null)
                {
                    existingEntry.Activities.Add(new JournalEntryActivity { UserActivity = userActivity });
                    continue;
                }

                // 3. Create new user activity
                var createUserActivityDto = new CreateUserActivityDto { Name = trimmedName };
                var newActivityDto = await _activityService.CreateUserActivityAsync(createUserActivityDto, userId);
                var newUserActivity = await _dbContext.UserActivities.FirstAsync(ua => ua.Id == newActivityDto.Id);
                existingEntry.Activities.Add(new JournalEntryActivity { UserActivity = newUserActivity });
            }

            // Update Secondary Emotions
            existingEntry.SecondaryEmotions.Clear();
            var emotions = await _dbContext.Emotions
                .Where(e => dto.SecondaryEmotionIds.Contains(e.Id))
                .ToListAsync();

            foreach (var emotion in emotions)
            {
                existingEntry.SecondaryEmotions.Add(new JournalEntryEmotion { Emotion = emotion });
            }

            await _dbContext.SaveChangesAsync();

            // Project to DTO instead of loading with .Include

            var updatedDto = await JournalEntryHelper
            .ProjectToDto(_dbContext.JournalEntries.Where(j => j.UserId == userId && j.Id == id))
            .FirstOrDefaultAsync();
            
            return updatedDto;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var journalEntry = await _dbContext.JournalEntries
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

            if (journalEntry == null)
                return false;

            _dbContext.JournalEntries.Remove(journalEntry);
            await _dbContext.SaveChangesAsync();
            return true;
        }

    }
}
