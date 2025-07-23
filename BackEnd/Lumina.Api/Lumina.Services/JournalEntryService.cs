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
            var journalEntries = await JournalEntryProjectionHelper
            .ProjectToDto(_dbContext.JournalEntries.Where(j => j.UserId == userId).OrderByDescending(j => j.CreatedAt))
            .ToListAsync();

            return journalEntries;
           
        }

        public async Task<JournalEntryDto?> GetByIdAsync(int id, string userId)
        {
            var journalEntry = await JournalEntryProjectionHelper
            .ProjectToDto(_dbContext.JournalEntries.Where(j => j.UserId == userId && j.Id == id))
            .FirstOrDefaultAsync();

            return journalEntry;

        }

        public async Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto dto, string userId)
        {
            var journalEntry = _mapper.Map<JournalEntry>(dto);
            journalEntry.UserId = userId;
            journalEntry.CreatedAt = DateTime.UtcNow;

            await JournalEntryHelper.AddTagsToEntryAsync(_tagService, userId, _dbContext, dto.TagNames, journalEntry);

            await JournalEntryHelper.AddActivitiessToEntryAsync(_activityService,userId,_dbContext,dto.ActivityNames, journalEntry);



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
            var savedEntryDto = await JournalEntryProjectionHelper
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
            await JournalEntryHelper.AddTagsToEntryAsync(_tagService, userId, _dbContext, dto.TagNames, existingEntry);
            existingEntry.Activities.Clear();
            await JournalEntryHelper.AddActivitiessToEntryAsync(_activityService, userId, _dbContext, dto.ActivityNames, existingEntry);


            

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

            var updatedDto = await JournalEntryProjectionHelper
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
