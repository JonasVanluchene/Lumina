using Lumina.Api.ASP.DTO;
using Lumina.Api.ASP.helpers;
using Lumina.DTO.Tag;
using Lumina.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumina.Api.ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagController> _logger;

        public TagController(ITagService tagService, ILogger<TagController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all System Tags 
        /// </summary>
        /// <returns>List of System tags</returns>
        [HttpGet("system")]
        [ProducesResponseType(typeof(List<TagDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<TagDto>>> GetAllSystemTags()
        {
            try
            {
                //if (!UserHelper.TryGetUserId(User, out var userId))
                //{
                //    _logger.LogWarning("Unauthorized access attempt - User ID not found in claims");
                //    return Unauthorized(new ErrorResponse
                //    {
                //        Message = "Authentication failed",
                //        Details = "User identity could not be verified from the authentication token"
                //    });
                //}

                var tags = await _tagService.GetAllSystemTagsAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system tags");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while retrieving system tags" });
            }
        }

        /// <summary>
        /// Gets all personal tags for the authenticated user
        /// </summary>
        /// <returns>List of personal tags</returns>
        [HttpGet("user")]
        [ProducesResponseType(typeof(List<UserTagDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserTagDto>>> GetAllUserTags()
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

                var tags = await _tagService.GetAllUserTagsAsync(userId);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving personal tags for user");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while retrieving user tags" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserTagDto>> GetUserTagById(int id)
        {
            // TODO: Implement when needed
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new personal tag
        /// </summary>
        /// <param name="model">The user tag data</param>
        /// <returns>The created user tag</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserTagDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserTagDto>> CreateUserTag([FromBody] CreateUserTagDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Invalid input data",
                        Details = string.Join("; ",
                            ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
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

                var tag = await _tagService.CreateUserTagAsync(model, userId);
                return CreatedAtAction(nameof(GetUserTagById), new { id = tag.Id }, tag);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data provided for user tag creation");
                return BadRequest(new ErrorResponse { Message = "Invalid data provided", Details = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating journal entry");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while creating the user tag" });
            }
        }

        /// <summary>
        /// Updates an existing user tag
        /// </summary>
        /// <param name="id">The user tag ID</param>
        /// <param name="model">The updated user tag data</param>
        /// <returns>The updated user tag</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UserTagDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserTagDto>> UpdateUserTag(int id, [FromBody] UpdateUserTagDto model)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Invalid user tag ID",
                        Details = "user tag ID must be a positive integer"
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
                    _logger.LogWarning("Unauthorized user tag update attempt for ID {Id}", id);
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Authentication failed",
                        Details = "User identity could not be verified from the authentication token"
                    });
                }

                var updated = await _tagService.UpdateUserTagAsync(id, model, userId);
                if (updated == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "user tag not found",
                        Details = $"No user tag found with ID {id} for the current user"
                    });
                }

                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data provided for user tag update {Id}", id);
                return BadRequest(new ErrorResponse { Message = "Invalid data provided", Details = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user tag {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while updating the user tag" });
            }
        }


        /// <summary>
        /// Deletes a personal tag
        /// </summary>
        /// <param name="id">The user tag ID</param>
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
                        Message = "Invalid user tag ID",
                        Details = "user tag ID must be a positive integer"
                    });
                }

                if (!UserHelper.TryGetUserId(User, out var userId))
                {
                    _logger.LogWarning("Unauthorized user tag deletion attempt for ID {Id}", id);
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Authentication failed",
                        Details = "User identity could not be verified from the authentication token"
                    });
                }

                await _tagService.DeleteUserTagAsync(id, userId);

                //if (!success)
                //{
                //    return NotFound(new ErrorResponse
                //    {
                //        Message = "Journal entry not found",
                //        Details = $"No journal entry found with ID {id} for the current user"
                //    });
                //}

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user tag {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while deleting the user tag" });
            }
        }
    }
}
