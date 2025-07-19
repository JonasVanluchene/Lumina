using Lumina.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Lumina.Api.ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }


        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.CreatedAt,
                user.LastLoginAt,
                user.IsActive
            });
        }

        //[HttpGet("{username}")]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetUserByUsername(string username)
        //{
        //    var user = await _userManager.FindByNameAsync(username);
        //    if (user == null)
        //        return NotFound();

        //    return Ok(new
        //    {
        //        user.Id,
        //        user.Email,
        //        user.FirstName,
        //        user.LastName,
        //        user.CreatedAt,
        //        user.LastLoginAt,
        //        user.IsActive
        //    });
        //}
    }
}
