using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HireZ.Services;
using HireZ.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace HireZ.Controllers
{
    [ApiController]
    [Route("api/interview")]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewService _interviewService;
        public InterviewController(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        [HttpPost("sessions")]
        [Authorize]
        public async Task<IActionResult> CreateSession([FromBody] CreateInterviewSessionRequest req)
        {
            if (req == null || req.ResumeId <= 0) return BadRequest("ResumeId is required.");

            var preferred = string.IsNullOrWhiteSpace(req.PreferredSource) ? "ai" : req.PreferredSource;
            var sessionId = await _interviewService.CreateInterviewSessionAndGenerateAsync(req.ResumeId, req.JobId, req.Count, preferred);
            return CreatedAtAction(nameof(GetSession), new { id = sessionId }, new { sessionId });
        }

        [HttpGet("sessions/{id}")]
        [Authorize]
        public async Task<IActionResult> GetSession(int id)
        {
            var session = await _interviewService.GetSessionAsync(id);
            if (session == null) return NotFound();

            var dto = new InterviewSessionDto
            {
                Id = session.Id,
                ResumeId = session.ResumeId,
                JobId = session.JobId,
                Status = session.Status,
                CreatedAt = session.CreatedAt,
                Questions = session.Questions.Select(q => new InterviewQuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    Category = q.Category,
                    Source = q.Source,
                    CreatedAt = q.CreatedAt
                }).ToList()
            };

            return Ok(dto);
        }
    }
}
