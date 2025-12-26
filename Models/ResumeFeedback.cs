using System;

namespace HireZ.Models
{
    public enum FeedbackType
    {
        AiSummary = 0,
        AtsScore = 1,
        Suggestions = 2
    }

    public class ResumeFeedback
    {
        public int Id { get; set; }
        public int ResumeId { get; set; }
        public FeedbackType Type { get; set; } = FeedbackType.AiSummary;
        public string FeedbackJson { get; set; } = null!;   // JSON blob produced by AI
        public double? AtsScore { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Resume? Resume { get; set; }
    }
}
