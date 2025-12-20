using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HireZ.Data;
using Microsoft.EntityFrameworkCore;

namespace HireZ.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ApplicationDbContext db, ILogger<ProfileController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            // prefer "userId" claim (user id set when generating JWT)
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized(new { message = "Invalid token (no userId claim)." });

            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { message = "Invalid user id in token." });
            }

            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound(new { message = "User not found." });

            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                role = user.Role,
                createdAt = user.CreatedAt
            });
        }
    }
}
