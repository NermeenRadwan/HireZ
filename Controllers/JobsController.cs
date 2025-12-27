using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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
        public JobsController(IJobService jobService) { _jobService = jobService; }

        [HttpPost]
        [Authorize]
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

        [HttpPost("{jobId}/match/{resumeId}")]
        [Authorize]
        public async Task<IActionResult> MatchResumeToJob(int jobId, int resumeId)
        {
            var result = await _jobService.MatchResumeToJobAsync(jobId, resumeId);
            return Ok(result);
        }
    }
}
