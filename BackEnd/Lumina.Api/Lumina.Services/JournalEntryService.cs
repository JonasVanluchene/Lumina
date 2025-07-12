using AutoMapper;
using Lumina.DTO.JournalEntry;
using Lumina.Repository;
using Lumina.Services.Interfaces;
using Lumina.Models;
using Microsoft.EntityFrameworkCore;

namespace Lumina.Services
{
    public class JournalEntryService : IJournalEntryService
    {
        private readonly LuminaDbContext _dbContext;
        private readonly IMapper _mapper;

        public JournalEntryService(LuminaDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<JournalEntryDto>> GetAllAsync(string userId)
        {
            var journalEntries = await _dbContext.JournalEntries
                .Include(j => j.PrimaryMood)
                .Include(j => j.Tags).ThenInclude(jt => jt.Tag)
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

            // Add Tags
            var tags = await _dbContext.Tags
                .Where(t => dto.TagIds.Contains(t.Id))
                .ToListAsync();

            //if (tags.Count != dto.TagIds.Count)
            //    throw new Exception("One or more TagIds are invalid.");

            foreach (var tag in tags)
            {
                journalEntry.Tags.Add(new JournalEntryTag { Tag = tag });
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

            // Reload with includes
            var savedEntry = await _dbContext.JournalEntries
                .Include(j => j.PrimaryMood)
                .Include(j => j.Tags).ThenInclude(t => t.Tag)
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
            existingEntry.UpdatedAt = DateTime.Now;

            // Update Tags
            existingEntry.Tags.Clear();
            var tags = await _dbContext.Tags
                .Where(t => dto.TagIds.Contains(t.Id))
                .ToListAsync();

            //if (tags.Count != dto.TagIds.Count)
            //    throw new Exception("One or more TagIds are invalid.");

            foreach (var tag in tags)
            {
                existingEntry.Tags.Add(new JournalEntryTag { Tag = tag });
            }

            // Update Secondary Emotions
            existingEntry.SecondaryEmotions.Clear();
            var emotions = await _dbContext.Emotions
                .Where(e => dto.SecondaryEmotionIds.Contains(e.Id))
                .ToListAsync();

            //if (emotions.Count != dto.SecondaryEmotionIds.Count)
            //    throw new Exception("One or more EmotionIds are invalid.");

            foreach (var emotion in emotions)
            {
                existingEntry.SecondaryEmotions.Add(new JournalEntryEmotion { Emotion = emotion });
            }

            await _dbContext.SaveChangesAsync();

            var updated = await _dbContext.JournalEntries
                .Include(j => j.PrimaryMood)
                .Include(j => j.Tags).ThenInclude(jt => jt.Tag)
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
