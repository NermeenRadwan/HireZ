using System.Collections.Generic;
using System.Threading.Tasks;
using HireZ.DTOs;

namespace HireZ.Services
{
    public interface IFeedbackService
    {
        /// <summary>
        /// Get feedback entries for a specific resume.
        /// </summary>
        Task<List<FeedbackDto>> GetFeedbackForResumeAsync(int resumeId);

        /// <summary>
        /// Get single feedback by id.
        /// </summary>
        Task<FeedbackDto?> GetFeedbackByIdAsync(int feedbackId);
    }
}
