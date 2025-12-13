using System;

namespace HireZ.Models
{
    public class InterviewSession
    {
        public int Id { get; set; }
        public int ResumeId { get; set; }
        public string QuestionsJson { get; set; } = null!;
        public string ScoresJson { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Resume? Resume { get; set; }
    }
}
