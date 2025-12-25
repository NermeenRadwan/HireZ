using System;

namespace HireZ.DTOs
{
    public class FeedbackDto
    {
        public int Id { get; set; }
        public int ResumeId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Optional textual content - not all projects use the same field name
        // Try to populate heuristically from common properties (FeedbackText, Content, Analysis)
        public string? Content { get; set; }

        // Optional source e.g., "AI", "Manual"
        public string? Source { get; set; }
    }
}
