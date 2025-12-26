using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HireZ.Services;
using Microsoft.AspNetCore.Authorization;

namespace HireZ.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analytics;
        public AnalyticsController(IAnalyticsService analytics)
        {
            _analytics = analytics;
        }

        /// <summary>
        /// GET /api/analytics/overview
        /// Returns high-level dashboard metrics.
        /// </summary>
        [HttpGet("overview")]
        [Authorize] // Require auth; tighten roles as needed
        public async Task<IActionResult> GetOverview()
        {
            var dto = await _analytics.GetOverviewAsync();
            return Ok(dto);
        }

        /// <summary>
        /// GET /api/analytics/trends?days=30
        /// Returns daily uploads vs analyses for the last 'days' days.
        /// </summary>
        [HttpGet("trends")]
        [Authorize]
        public async Task<IActionResult> GetTrends([FromQuery] int days = 30)
        {
            var points = await _analytics.GetResumeTrendsAsync(days);
            return Ok(points);
        }
    }
}
