using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeX.Dashboard.Core.Entities;
using RealtimeX.Dashboard.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RealtimeX.Dashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (success, token) = await _authService.LoginAsync(request.Username, request.Password);
            if (!success)
            {
                return Unauthorized();
            }

            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var success = await _authService.RegisterAsync(user, request.Password);
            if (!success)
            {
                return BadRequest("Username already exists");
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = int.Parse(User.FindFirst("id")?.Value);
            var success = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            if (!success)
            {
                return BadRequest("Invalid current password");
            }

            return Ok();
        }
    }

    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = null!;
        
        [Required]
        public string Password { get; set; } = null!;
    }

    public class RegisterRequest
    {
        [Required]
        public string Username { get; set; } = null!;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        [Required]
        public string Password { get; set; } = null!;
        
        [Required]
        public string FirstName { get; set; } = null!;
        
        [Required]
        public string LastName { get; set; } = null!;
    }

    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;
        
        [Required]
        public string NewPassword { get; set; } = null!;
    }
} 