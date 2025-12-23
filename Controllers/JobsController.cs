using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HireZ.Services;
using HireZ.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HireZ.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;
        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpPost]
        [Authorize] // enforce JWT; add Roles parameter later if desired
        public async Task<IActionResult> Create([FromBody] CreateJobRequest request)
        {
            var result = await _jobService.CreateJobAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var job = await _jobService.GetJobAsync(id);
            if (job == null) return NotFound();
            return Ok(job);
        }

        /// <summary>
        /// Match a resume to a job and return ATS result.
        /// </summary>
        [HttpPost("{jobId}/match/{resumeId}")]
        [Authorize]
        public async Task<IActionResult> MatchResumeToJob(int jobId, int resumeId)
        {
            var result = await _jobService.MatchResumeToJobAsync(jobId, resumeId);
            return Ok(result);
        }
    }
}
