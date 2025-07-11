using AutoMapper;
using Lumina.Api.ASP.DTO;
using Lumina.Api.ASP.DTO.Auth;
using Lumina.Models;
using Lumina.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly JwtTokenService _jwtTokenService;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<AuthController> _logger;
    private readonly IMapper _mapper;

    public AuthController(UserManager<User> userManager, JwtTokenService jwtTokenService, SignInManager<User> signInManager, ILogger<AuthController> logger, IMapper mapper)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _signInManager = signInManager;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
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

            var user = _mapper.Map<User>(dto);
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Registration failed",
                    Details = string.Join("; ", result.Errors.Select(e => e.Description))
                });
            }

            var token = _jwtTokenService.CreateToken(user);
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token
            };

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during registration");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse { Message = "An unexpected error occurred while registering the user" });
        }
    }

    /// <summary>
    /// Login using email or username
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
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

            var user = await _userManager.FindByEmailAsync(dto.Identifier)
                        ?? await _userManager.FindByNameAsync(dto.Identifier);

            if (user == null)
            {
                return Unauthorized(new ErrorResponse
                {
                    Message = "Authentication failed",
                    Details = "Invalid email/username or password"
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new ErrorResponse
                {
                    Message = "Authentication failed",
                    Details = "Invalid email/username or password"
                });
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var token = _jwtTokenService.CreateToken(user);
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token
            };

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse { Message = "An unexpected error occurred while logging in" });
        }
    }
}
