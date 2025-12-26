using System.Threading.Tasks;
using HireZ.DTOs;

namespace HireZ.Services
{
    public interface IJobService
    {
        Task<JobDto> CreateJobAsync(CreateJobRequest request);
        Task<JobDto?> GetJobAsync(int jobId);
        /// <summary>
        /// Match a resume (by resumeId) against a job (jobId) and return ATS result.
        /// This will also persist matched keywords records if MatchedKeyword entity exists.
        /// </summary>
        Task<AtsResultDto> MatchResumeToJobAsync(int jobId, int resumeId);
    }
}
