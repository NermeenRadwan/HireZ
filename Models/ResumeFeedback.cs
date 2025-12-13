using System;

namespace HireZ.Models
{
    public class ResumeFeedback
    {
        public int Id { get; set; }
        public int ResumeId { get; set; }
        public string FeedbackJson { get; set; } = null!;   // store structured AI feedback here
        public double? AtsScore { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Resume? Resume { get; set; }
    }
}
