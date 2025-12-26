using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HireZ.Models
{
    public class InterviewQuestion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InterviewSessionId { get; set; }

        [Required]
        public string QuestionText { get; set; } = "";

        // e.g., "technical", "behavioral", "scenario"
        [MaxLength(50)]
        public string? Category { get; set; }

        // Where the question came from: "AI" or "Heuristic"
        [MaxLength(20)]
        public string Source { get; set; } = "AI";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public InterviewSession? InterviewSession { get; set; }
    }
}
