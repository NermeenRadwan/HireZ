using System.Collections.Generic;
using System.Threading.Tasks;
using HireZ.Models;

namespace HireZ.Services
{
    public interface IInterviewService
    {
        Task<List<string>> GenerateInterviewQuestionsAsync(string resumeText, string? jobDescription = null, int count = 8);
        Task<List<string>> GenerateInterviewQuestionsAsync(int resumeId, int? jobId = null, int count = 8);
        Task<int> CreateInterviewSessionAndGenerateAsync(int resumeId, int? jobId, int count = 8, string preferredSource = "ai");
        Task<InterviewSession?> GetSessionAsync(int sessionId);
    }
}
