using AutoMapper;
using Lumina.Api.ASP.DTO;
using Lumina.Api.ASP.DTO.Auth;
using Lumina.Models;
using Lumina.Services;
using Lumina.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumina.Api.ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;

        public AuthController(UserManager<User> userManager, JwtTokenService jwtTokenService, IRefreshTokenService refreshTokenService,
            SignInManager<User> signInManager, ILogger<AuthController> logger, IMapper mapper)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _refreshTokenService = refreshTokenService;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)] // model validation errors
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)] // registration logic errors
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
                        Details = string.Join("; ",
                            ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                    });
                }


                // TODO: implement "Account Already Exists" Flow: try not to expose emailaddresses from already registered users
                // TODO: check token return in response headers (not safe)

                // Checks for duplicates internally, but don't reveal which one failed
                var existingUserByEmail = await _userManager.FindByEmailAsync(dto.Email);
                var username = string.IsNullOrWhiteSpace(dto.UserName) ? dto.Email : dto.UserName;
                var existingUserByName = await _userManager.FindByNameAsync(username);

                if (existingUserByEmail != null || existingUserByName != null)
                {
                    // Generic message - don't reveal which field was duplicate (for privacy)
                    return BadRequest(new ErrorResponse
                    {
                        Message =
                            "Registration failed. Please check your information and try again. Email may already have registered"
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
                // Set JWT as HttpOnly cookie
                Response.Cookies.Append("access_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Set to true in production
                    SameSite = SameSiteMode.Strict, // Or Lax, depending on your needs
                    Expires = DateTimeOffset.UtcNow.AddMinutes(60) // Match your token expiry
                });
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = token  //This return is for swagger (not safe)
                };


                var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user);
                Response.Cookies.Append("refresh_token", refreshToken.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = refreshToken.ExpiresAt
                });
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
                        Details = string.Join("; ",
                            ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
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
                // Set JWT as HttpOnly cookie
                Response.Cookies.Append("access_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Set to true in production
                    SameSite = SameSiteMode.Strict, // Or Lax, depending on your needs
                    Expires = DateTimeOffset.UtcNow.AddMinutes(60) // Match your token expiry
                });
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = token  //TODO: Change token return in response header
                };


                var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user);
                Response.Cookies.Append("refresh_token", refreshToken.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = refreshToken.ExpiresAt
                });
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Message = "An unexpected error occurred while logging in" });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshTokenValue = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshTokenValue))
                return Unauthorized(new ErrorResponse { Message = "No refresh token provided" });

            var tokenEntity = await _refreshTokenService.GetValidRefreshTokenAsync(refreshTokenValue);
            if (tokenEntity == null)
                return Unauthorized(new ErrorResponse { Message = "Invalid or expired refresh token" });

            await _refreshTokenService.InvalidateRefreshTokenAsync(tokenEntity);

            var newJwt = _jwtTokenService.CreateToken(tokenEntity.User);
            var newRefreshToken = await _refreshTokenService.CreateRefreshTokenAsync(tokenEntity.User);

            Response.Cookies.Append("refresh_token", newRefreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newRefreshToken.ExpiresAt
            });

            return Ok(new { token = newJwt });
        }
    }
}
