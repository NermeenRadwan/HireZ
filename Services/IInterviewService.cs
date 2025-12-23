using System.Collections.Generic;
using System.Threading.Tasks;

namespace HireZ.Services
{
    public interface IInterviewService
    {
        /// <summary>
        /// Generate interview questions targeted at a resume and optional job description.
        /// </summary>
        Task<List<string>> GenerateInterviewQuestionsAsync(string resumeText, string? jobDescription = null, int count = 8);

        /// <summary>
        /// Convenience: use resumeId & jobId to create questions (loads resume text/job from db).
        /// </summary>
        Task<List<string>> GenerateInterviewQuestionsAsync(int resumeId, int? jobId = null, int count = 8);
    }
}
