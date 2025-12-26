using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HireZ.Services;
using HireZ.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HireZ.Controllers
{
    [ApiController]
    [Route("api/resume/{resumeId}/feedback")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedback;
        public FeedbackController(IFeedbackService feedback)
        {
            _feedback = feedback;
        }

        /// <summary>
        /// GET /api/resume/{resumeId}/feedback
        /// Returns list of feedback entries for resume.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetForResume([FromRoute] int resumeId)
        {
            var list = await _feedback.GetFeedbackForResumeAsync(resumeId);
            return Ok(list);
        }

        /// <summary>
        /// GET /api/resume/{resumeId}/feedback/{id}
        /// Get details of one feedback entry.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int resumeId, [FromRoute] int id)
        {
            var f = await _feedback.GetFeedbackByIdAsync(id);
            if (f == null || f.ResumeId != resumeId) return NotFound();
            return Ok(f);
        }

        /// <summary>
        /// POST /api/resume/{resumeId}/feedback
        /// Create a new feedback entry for a resume.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromRoute] int resumeId, [FromBody] CreateFeedbackRequest request)
        {
            if (request == null) return BadRequest(new { message = "Feedback data is required" });

            var feedback = await _feedback.CreateFeedbackAsync(resumeId, request);
            return CreatedAtAction(nameof(GetById), new { resumeId, id = feedback.Id }, feedback);
        }
    }
}
