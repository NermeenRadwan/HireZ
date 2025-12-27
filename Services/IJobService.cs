using System.Threading.Tasks;
using HireZ.DTOs;

namespace HireZ.Services
{
    public interface IJobService
    {
        Task<JobDto> CreateJobAsync(CreateJobRequest request);
        Task<JobDto?> GetJobAsync(int jobId);
        Task<AtsResultDto> MatchResumeToJobAsync(int jobId, int resumeId);
    }
}
