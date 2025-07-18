using AutoMapper;
using Lumina.DTO.JournalEntry;
using Lumina.DTO.Tag;
using Lumina.DTO.Activity;
using Lumina.Repository;
using Lumina.Services.Interfaces;
using Lumina.Models;
using Microsoft.EntityFrameworkCore;

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
            var journalEntries = await _dbContext.JournalEntries
                .Include(j => j.PrimaryMood)
                .Include(j => j.Tags).ThenInclude(jt => jt.Tag)
                .Include(j => j.Tags).ThenInclude(jt => jt.UserTag)
                .Include(j => j.SecondaryEmotions).ThenInclude(js => js.Emotion)
                .Include(j => j.Activities).ThenInclude(ja => ja.Activity)
                .Where(j => j.UserId == userId)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<JournalEntryDto>>(journalEntries);
        }

        public async Task<JournalEntryDto?> GetByIdAsync(int id, string userId)
        {
            var journalEntry = await _dbContext.JournalEntries
                .Include(j => j.PrimaryMood)
                .Include(j => j.Tags).ThenInclude(jt => jt.Tag)
                .Include(j => j.Tags).ThenInclude(jt => jt.UserTag)
                .Include(j => j.SecondaryEmotions).ThenInclude(js => js.Emotion)
                .Include(j => j.Activities).ThenInclude(ja => ja.Activity)
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

            if (journalEntry is null)
                return null;

            var dto = _mapper.Map<JournalEntryDto>(journalEntry);
            return dto;
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
                }

                // 3. Create new user activity
                var createUserActivityDto = new CreateUserActivityDto { Name = trimmedName };
                var newActivityDto = await _activityService.CreateUserActivityAsync(createUserActivityDto, userId);
                var newUserActivity = await _dbContext.UserActivities.FirstAsync(ut => ut.Id == newActivityDto.Id);
                journalEntry.Activities.Add(new JournalEntryActivity { UserActivity = newUserActivity });
            }


            //// Add System Tags
            //var systemTags = await _dbContext.Tags
            //    .Where(t => dto.TagIds.Contains(t.Id))
            //    .ToListAsync();

            //foreach (var tag in systemTags)
            //{
            //    journalEntry.Tags.Add(new JournalEntryTag { Tag = tag });
            //}

            //// Add existing User Tags (validate they belong to the current user)
            //var userTags = await _dbContext.UserTags
            //    .Where(ut => dto.UserTagIds.Contains(ut.Id) && ut.UserId == userId && ut.IsActive)
            //    .ToListAsync();

            //foreach (var userTag in userTags)
            //{
            //    journalEntry.Tags.Add(new JournalEntryTag { UserTag = userTag });
            //}

            ////Create and add new User Tags
            //foreach (var newTagName in dto.NewUserTagNames)
            //{
            //    var trimmedName = newTagName.Trim();
            //    var existingTag = await _dbContext.UserTags
            //        .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.Name == trimmedName && ut.IsActive);

            //    UserTag userTag;
            //    if (existingTag != null)
            //    {
            //        userTag = existingTag;
            //    }
            //    else
            //    {
            //        var createUserTagDto = new CreateUserTagDto { Name = trimmedName };
            //        var newTagDto = await _tagService.CreateUserTagAsync(createUserTagDto, userId);
            //        userTag = await _dbContext.UserTags.FirstAsync(ut => ut.Id == newTagDto.Id);
            //    }
            //    journalEntry.Tags.Add(new JournalEntryTag { UserTag = userTag });
            //}

            //// Add System Activities
            //var systemActivities = await _dbContext.Activities
            //    .Where(t => dto.ActivityIds.Contains(t.Id))
            //    .ToListAsync();

            //foreach (var activity in systemActivities)
            //{
            //    journalEntry.Activities.Add(new JournalEntryActivity { Activity = activity });
            //}

            //// Add existing User Activities (validate they belong to the current user)
            //var userActivities = await _dbContext.UserActivities
            //    .Where(ua => dto.UserActivityIds.Contains(ua.Id) && ua.UserId == userId && ua.IsActive)
            //    .ToListAsync();

            //foreach (var userActivity in userActivities)
            //{
            //    journalEntry.Activities.Add(new JournalEntryActivity { UserActivity = userActivity });
            //}

            ////Create and add new User Activites
            //foreach (var newActivityName in dto.NewUserActivityNames)
            //{
            //    var trimmedName = newActivityName.Trim();
            //    var existingActivity = await _dbContext.UserActivities
            //        .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.Name == trimmedName && ua.IsActive);

            //    UserActivity userActivity;
            //    if (existingActivity != null)
            //    {
            //        userActivity = existingActivity;
            //    }
            //    else
            //    {
            //        var createUserActivityDto = new CreateUserActivityDto { Name = trimmedName };
            //        var newActivityDto = await _activityService.CreateUserActivityAsync(createUserActivityDto, userId);
            //        userActivity = await _dbContext.UserActivities.FirstAsync(ua => ua.Id == newActivityDto.Id);
            //    }
            //    journalEntry.Activities.Add(new JournalEntryActivity { UserActivity = userActivity });
            //}

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

            // Reload with includes
            var savedEntry = await _dbContext.JournalEntries
                .Include(j => j.PrimaryMood)
                .Include(j => j.Tags).ThenInclude(t => t.Tag)
                .Include(j => j.Tags).ThenInclude(t => t.UserTag)
                .Include(j => j.Activities).ThenInclude(ja => ja.Activity)
                .Include(j => j.Activities).ThenInclude(ja => ja.UserActivity)
                .Include(j => j.SecondaryEmotions).ThenInclude(e => e.Emotion)
                .FirstAsync(j => j.Id == journalEntry.Id);

            return _mapper.Map<JournalEntryDto>(savedEntry);
        }

        public async Task<JournalEntryDto?> UpdateAsync(int id, UpdateJournalEntryDto dto, string userId)
        {
            var existingEntry = await _dbContext.JournalEntries
                .Include(j => j.Tags)
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

            // Reload with includes
            var updated = await _dbContext.JournalEntries
                .Include(j => j.PrimaryMood)
                .Include(j => j.Tags).ThenInclude(jt => jt.Tag)
                .Include(j => j.Tags).ThenInclude(jt => jt.UserTag)
                .Include(j => j.Activities).ThenInclude(ja => ja.Activity)
                .Include(j => j.Activities).ThenInclude(ja => ja.UserActivity)
                .Include(j => j.SecondaryEmotions).ThenInclude(js => js.Emotion)
                .FirstAsync(j => j.Id == id);

            return _mapper.Map<JournalEntryDto>(updated);
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
