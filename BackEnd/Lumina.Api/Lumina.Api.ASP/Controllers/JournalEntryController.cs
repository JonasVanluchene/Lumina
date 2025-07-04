using System.Security.Claims;
using Lumina.DTO.JournalEntry;
using Lumina.Services;
using Lumina.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lumina.Api.ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JournalEntryController : ControllerBase
    {
        private readonly IJournalEntryService _journalEntryService;

        public JournalEntryController(IJournalEntryService journalEntryService)
        {
            _journalEntryService = journalEntryService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JournalEntryDto>> GetJournalEntryById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();

            var journalEntry = await _journalEntryService.GetByIdAsync(id, userId);

            if (journalEntry == null)
                return NotFound();

            return Ok(journalEntry);
        }

        [HttpPost]
        public async Task<ActionResult<JournalEntryDto>> CreateJournalEntry([FromBody] CreateJournalEntryDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            var journalEntry = await _journalEntryService.CreateAsync(model, userId);
            return CreatedAtAction(
                nameof(GetJournalEntryById),
                new { id = journalEntry.Id },
                journalEntry);
        }
    }
}
