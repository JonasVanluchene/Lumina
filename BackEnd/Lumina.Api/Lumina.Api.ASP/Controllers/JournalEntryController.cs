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
        [ProducesResponseType(typeof(JournalEntryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<JournalEntryDto>> CreateJournalEntry([FromBody] CreateJournalEntryDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }
            try
            {
                var journalEntry = await _journalEntryService.CreateAsync(model, userId);
                return CreatedAtAction(nameof(GetJournalEntryById), new { id = journalEntry.Id }, journalEntry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<JournalEntryDto>> UpdateJournalEntry(int id, [FromBody] UpdateJournalEntryDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            try
            {
                var updated = await _journalEntryService.UpdateAsync(id, model, userId);
                if (updated == null)
                    return NotFound();
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
