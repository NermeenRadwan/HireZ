using HireZ.DTOs.Auth;
using HireZ.Services;
using Microsoft.AspNetCore.Mvc;

namespace HireZ.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var (success, error) = await _userService.RegisterAsync(request);
            if (!success) return BadRequest(new { message = error });

            return Ok(new { message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (success, response, error) = await _userService.AuthenticateAsync(request);
            if (!success) return Unauthorized(new { message = error });

            return Ok(response);
        }
    }
}
