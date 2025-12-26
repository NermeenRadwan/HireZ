using HireZ.DTOs.Resume;

namespace HireZ.Services
{
    public interface IResumeService
    {
        Task<int> UploadAsync(int userId, Stream fileStream, string fileName);
        Task<string?> GetExtractedTextAsync(int resumeId);
        Task<ResumeDto?> GetResumeAsync(int resumeId);
        Task QueueAnalysisAsync(int resumeId);
        Task<List<ResumeDto>> GetUserResumesAsync(int userId);
    }
}
