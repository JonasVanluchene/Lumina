using Lumina.Api.ASP.DTO;
using Lumina.Api.ASP.helpers;
using Lumina.DTO.Activity;
using Lumina.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Lumina.Api.ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<ActivitiesController> _logger;

        public ActivitiesController(IActivityService activityService, ILogger<ActivitiesController> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all System Activities
        /// </summary>
        /// <returns>List of System Activities</returns>
        [HttpGet("system")]
        [ProducesResponseType(typeof(List<ActivityDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ActivityDto>>> GetAllSystemActivities()
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

                var activities = await _activityService.GetAllSystemActivitiesAsync();
                return Ok(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system Activities");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while retrieving system activities" });
            }
        }

        /// <summary>
        /// Gets all personal activities for the authenticated user
        /// </summary>
        /// <returns>List of personal activities</returns>
        [HttpGet("user")]
        [ProducesResponseType(typeof(List<UserActivityDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserActivityDto>>> GetAllUserActivities()
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

                var activities = await _activityService.GetAllUserActivitiesAsync(userId);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving personal activities for user");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while retrieving user activities" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserActivityDto>> GetUserActivityById(int id)
        {
            // TODO: Implement when needed
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new personal activity
        /// </summary>
        /// <param name="model">The user activity data</param>
        /// <returns>The created user activity</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserActivityDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserActivityDto>> CreateUserActivity([FromBody] CreateUserActivityDto model)
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
                    _logger.LogWarning("Unauthorized activity creation attempt");
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Authentication failed",
                        Details = "User identity could not be verified from the authentication token"
                    });
                }

                var activity = await _activityService.CreateUserActivityAsync(model, userId);
                return CreatedAtAction(nameof(GetUserActivityById), new { id = activity.Id }, activity);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data provided for user activity creation");
                return BadRequest(new ErrorResponse { Message = "Invalid data provided", Details = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating journal entry");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while creating the user activity" });
            }
        }

        /// <summary>
        /// Updates an existing user activity
        /// </summary>
        /// <param name="id">The user activity ID</param>
        /// <param name="model">The updated user activity data</param>
        /// <returns>The updated user activity</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UserActivityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserActivityDto>> UpdateUserActivity(int id, [FromBody] UpdateUserActivityDto model)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Invalid user activity ID",
                        Details = "user activity ID must be a positive integer"
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
                    _logger.LogWarning("Unauthorized user activity update attempt for ID {Id}", id);
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Authentication failed",
                        Details = "User identity could not be verified from the authentication token"
                    });
                }

                var updated = await _activityService.UpdateUserActivityAsync(id, model, userId);
                if (updated == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "user activity not found",
                        Details = $"No user activity found with ID {id} for the current user"
                    });
                }

                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data provided for user activity update {Id}", id);
                return BadRequest(new ErrorResponse { Message = "Invalid data provided", Details = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user activity {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while updating the user activity" });
            }
        }


        /// <summary>
        /// Deletes a personal activity
        /// </summary>
        /// <param name="id">The user activity ID</param>
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
                        Message = "Invalid user activity ID",
                        Details = "user activity ID must be a positive integer"
                    });
                }

                if (!UserHelper.TryGetUserId(User, out var userId))
                {
                    _logger.LogWarning("Unauthorized user activity deletion attempt for ID {Id}", id);
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Authentication failed",
                        Details = "User identity could not be verified from the authentication token"
                    });
                }

                await _activityService.DeleteUserActivityAsync(id, userId);

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
                _logger.LogError(ex, "Error deleting user activity {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while deleting the user activity" });
            }
        }
    }
}
