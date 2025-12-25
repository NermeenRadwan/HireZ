using System.Collections.Generic;
using System.Threading.Tasks;
using HireZ.Models;

namespace HireZ.Services
{
    /// <summary>
    /// Contract for interview-related features:
    /// - heuristic or AI-backed question generation
    /// - create/persist interview sessions and questions
    /// - retrieve interview session details
    /// </summary>
    public interface IInterviewService
    {
        /// <summary>
        /// Generate interview questions from raw resume text and optional job description.
        /// </summary>
        Task<List<string>> GenerateInterviewQuestionsAsync(string resumeText, string? jobDescription = null, int count = 8);

        /// <summary>
        /// Generate interview questions by loading resume (and optionally job) from the database.
        /// </summary>
        Task<List<string>> GenerateInterviewQuestionsAsync(int resumeId, int? jobId = null, int count = 8);

        /// <summary>
        /// Create an InterviewSession, generate questions (AI or heuristic), persist them,
        /// and return the created session id.
        /// </summary>
        Task<int> CreateInterviewSessionAndGenerateAsync(int resumeId, int? jobId, int count = 8, string preferredSource = "ai");

        /// <summary>
        /// Retrieve InterviewSession by id including its persisted questions.
        /// Returns null if not found.
        /// </summary>
        Task<InterviewSession?> GetSessionAsync(int sessionId);
    }
}
