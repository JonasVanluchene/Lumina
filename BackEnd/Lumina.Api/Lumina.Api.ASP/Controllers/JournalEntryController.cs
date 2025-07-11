using Lumina.Api.ASP.DTO;
using Lumina.Api.ASP.helpers;
using Lumina.DTO.JournalEntry;
using Lumina.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumina.Api.ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JournalEntryController : ControllerBase
    {
        private readonly IJournalEntryService _journalEntryService;
        private readonly ILogger<JournalEntryController> _logger;

        public JournalEntryController(IJournalEntryService journalEntryService, ILogger<JournalEntryController> logger)
        {
            _journalEntryService = journalEntryService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all journal entries for the authenticated user
        /// </summary>
        /// <returns>List of journal entries</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<JournalEntryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<JournalEntryDto>>> GetAll()
        {
            try
            {
                if (!UserHelper.TryGetUserId(User, out var userId))
                {
                    _logger.LogWarning("Unauthorized access attempt - User ID not found in claims");
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Authentication failed",
                        Details = "User identity could not be verified from the authentication token"
                    });
                }

                var journalEntries = await _journalEntryService.GetAllAsync(userId);

                return Ok(journalEntries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving journal entries for user");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while retrieving journal entries" });
            }
        }

        /// <summary>
        /// Gets a specific journal entry by ID
        /// </summary>
        /// <param name="id">The journal entry ID</param>
        /// <returns>The journal entry</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(JournalEntryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JournalEntryDto>> GetJournalEntryById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Invalid journal entry ID",
                        Details = "Journal entry ID must be a positive integer"
                    });
                }

                if (!UserHelper.TryGetUserId(User, out var userId))
                {
                    _logger.LogWarning("Unauthorized access attempt for journal entry {Id}", id);
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Authentication failed",
                        Details = "User identity could not be verified from the authentication token"
                    });
                }

                var journalEntry = await _journalEntryService.GetByIdAsync(id, userId);

                if (journalEntry == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Journal entry not found",
                        Details = $"No journal entry found with ID {id} for the current user"
                    });
                }

                return Ok(journalEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving journal entry {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while retrieving the journal entry" });
            }
        }

        /// <summary>
        /// Creates a new journal entry
        /// </summary>
        /// <param name="model">The journal entry data</param>
        /// <returns>The created journal entry</returns>
        [HttpPost]
        [ProducesResponseType(typeof(JournalEntryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JournalEntryDto>> CreateJournalEntry([FromBody] CreateJournalEntryDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Invalid input data",
                        Details = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                    });
                }

                if (!UserHelper.TryGetUserId(User, out var userId))
                {
                    _logger.LogWarning("Unauthorized journal entry creation attempt");
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Authentication failed",
                        Details = "User identity could not be verified from the authentication token"
                    });
                }

                var journalEntry = await _journalEntryService.CreateAsync(model, userId);
                return CreatedAtAction(nameof(GetJournalEntryById), new { id = journalEntry.Id }, journalEntry);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data provided for journal entry creation");
                return BadRequest(new ErrorResponse { Message = "Invalid data provided", Details = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating journal entry");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while creating the journal entry" });
            }
        }

        /// <summary>
        /// Updates an existing journal entry
        /// </summary>
        /// <param name="id">The journal entry ID</param>
        /// <param name="model">The updated journal entry data</param>
        /// <returns>The updated journal entry</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(JournalEntryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JournalEntryDto>> UpdateJournalEntry(int id, [FromBody] UpdateJournalEntryDto model)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Invalid journal entry ID",
                        Details = "Journal entry ID must be a positive integer"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Invalid input data",
                        Details = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                    });
                }

                if (!UserHelper.TryGetUserId(User, out var userId))
                {
                    _logger.LogWarning("Unauthorized journal entry update attempt for ID {Id}", id);
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Authentication failed",
                        Details = "User identity could not be verified from the authentication token"
                    });
                }

                var updated = await _journalEntryService.UpdateAsync(id, model, userId);
                if (updated == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Journal entry not found",
                        Details = $"No journal entry found with ID {id} for the current user"
                    });
                }

                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data provided for journal entry update {Id}", id);
                return BadRequest(new ErrorResponse { Message = "Invalid data provided", Details = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating journal entry {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while updating the journal entry" });
            }
        }

        /// <summary>
        /// Deletes a journal entry
        /// </summary>
        /// <param name="id">The journal entry ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Invalid journal entry ID",
                        Details = "Journal entry ID must be a positive integer"
                    });
                }

                if (!UserHelper.TryGetUserId(User, out var userId))
                {
                    _logger.LogWarning("Unauthorized journal entry deletion attempt for ID {Id}", id);
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Authentication failed",
                        Details = "User identity could not be verified from the authentication token"
                    });
                }

                var success = await _journalEntryService.DeleteAsync(id, userId);

                if (!success)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Journal entry not found",
                        Details = $"No journal entry found with ID {id} for the current user"
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting journal entry {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while deleting the journal entry" });
            }
        }
    }
}
