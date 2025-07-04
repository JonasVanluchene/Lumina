using AutoMapper;
using Lumina.DTO.JournalEntry;
using Lumina.Repository;
using Lumina.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<JournalEntryDto?> GetByIdAsync(int id, string userId)
        {
            var journalEntry = await _dbContext.JournalEntries
                .Include(j => j.PrimaryMood)
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
            journalEntry.CreatedAt = DateTime.Now;
            await _dbContext.JournalEntries.AddAsync(journalEntry);
            await _dbContext.SaveChangesAsync();

            // Map back to a DTO for return
            var resultDto = _mapper.Map<JournalEntryDto>(journalEntry);
            return resultDto;
        }

        
    }
}
