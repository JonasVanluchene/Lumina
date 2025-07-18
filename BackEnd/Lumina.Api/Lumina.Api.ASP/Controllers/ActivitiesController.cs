using Lumina.Api.ASP.DTO;
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

                var tags = await _activityService.GetAllSystemActivitiesAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system Activities");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An error occurred while retrieving system tags" });
            }
        }
    }
}
