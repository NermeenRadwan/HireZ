using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HireZ.Services;
using HireZ.DTOs.Resume;
using System.Security.Claims;

namespace HireZ.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumeController : ControllerBase
    {
        private readonly IResumeService _resumeService;

        public ResumeController(IResumeService resumeService)
        {
            _resumeService = resumeService;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(new { message = "File is required" });

            // get user id from claims (as earlier)
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;
            var resumeId = await _resumeService.UploadAsync(userId, ms, file.FileName);

            var response = new UploadResponse { ResumeId = resumeId, FileName = file.FileName };
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var resume = await _resumeService.GetResumeAsync(id);
            if (resume == null) return NotFound();
            return Ok(resume);
        }

        [HttpPost("{id}/reprocess")]
        [Authorize]
        public async Task<IActionResult> Reprocess(int id)
        {
            await _resumeService.QueueAnalysisAsync(id);
            return Accepted(new { message = "Reprocessing queued." });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId" || c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var resumes = await _resumeService.GetUserResumesAsync(userId);
            return Ok(resumes);
        }
    }
}
