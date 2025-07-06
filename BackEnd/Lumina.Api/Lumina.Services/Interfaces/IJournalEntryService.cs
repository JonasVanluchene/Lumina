using Lumina.DTO.JournalEntry;


namespace Lumina.Services.Interfaces
{
    public interface IJournalEntryService
    {
        Task<JournalEntryDto?> GetByIdAsync(int id, string userId);
        Task<IEnumerable<JournalEntryDto>> GetAllAsync(string userId);
        Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto dto, string userId);
        Task<JournalEntryDto?> UpdateAsync(int id, UpdateJournalEntryDto dto, string userId);
        Task<bool> DeleteAsync(int id, string userId);
    }
}
