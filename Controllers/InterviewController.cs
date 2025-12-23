using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HireZ.Services;
using Microsoft.AspNetCore.Authorization;

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

        /// <summary>
        /// Generate interview questions by passing resumeId and optional jobId.
        /// </summary>
        [HttpPost("generate")]
        [Authorize]
        public async Task<IActionResult> Generate([FromQuery] int resumeId, [FromQuery] int? jobId, [FromQuery] int count = 8)
        {
            var questions = await _interviewService.GenerateInterviewQuestionsAsync(resumeId, jobId, count);
            return Ok(new { Questions = questions });
        }

        /// <summary>
        /// Generate questions from raw text payload (client has resume text).
        /// </summary>
        public class GenerateFromTextRequest
        {
            public string ResumeText { get; set; } = "";
            public string? JobDescription { get; set; }
            public int Count { get; set; } = 8;
        }

        [HttpPost("generate/from-text")]
        [Authorize]
        public async Task<IActionResult> GenerateFromText([FromBody] GenerateFromTextRequest req)
        {
            var questions = await _interviewService.GenerateInterviewQuestionsAsync(req.ResumeText ?? "", req.JobDescription, req.Count);
            return Ok(new { Questions = questions });
        }
    }
}
