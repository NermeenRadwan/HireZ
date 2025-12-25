using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireZ.DTOs;

namespace HireZ.Services
{
    public interface IAnalyticsService
    {
        /// <summary>
        /// High-level overview metrics for dashboards.
        /// </summary>
        Task<AnalyticsOverviewDto> GetOverviewAsync();

        /// <summary>
        /// Get daily trend points (uploads vs analyses) for last N days.
        /// </summary>
        Task<List<TrendPointDto>> GetResumeTrendsAsync(int days);
    }
}
