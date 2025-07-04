using Lumina.DTO.JournalEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Services.Interfaces
{
    public interface IJournalEntryService
    {
        Task<JournalEntryDto> GetByIdAsync(int id, string userId);
        Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto dto, string userId);
    }
}
