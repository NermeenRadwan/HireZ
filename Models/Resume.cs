using System;

namespace HireZ.Models
{
    public class Resume
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
        public ResumeText? ResumeText { get; set; }
        public ICollection<ResumeFeedback> Feedbacks { get; set; } = new List<ResumeFeedback>();
        public ICollection<MatchedKeyword> MatchedKeywords { get; set; } = new List<MatchedKeyword>();
        public ICollection<InterviewSession> InterviewSessions { get; set; } = new List<InterviewSession>();
    }
}
