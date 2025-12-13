using System;

namespace HireZ.Models
{
    public class ResumeText
    {
        public int Id { get; set; }
        public int ResumeId { get; set; }
        public string Text { get; set; } = null!;
        public DateTime ExtractedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Resume? Resume { get; set; }
    }
}
