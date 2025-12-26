using System;
using System.Collections.Generic;

namespace HireZ.DTOs
{
    public class InterviewQuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = "";
        public string? Category { get; set; }
        public string Source { get; set; } = "AI";
        public DateTime CreatedAt { get; set; }
    }

    public class InterviewSessionDto
    {
        public int Id { get; set; }
        public int ResumeId { get; set; }
        public int? JobId { get; set; }
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public List<InterviewQuestionDto> Questions { get; set; } = new();
    }
}
