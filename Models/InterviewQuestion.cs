using System;
using System.ComponentModel.DataAnnotations;

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

        [MaxLength(50)]
        public string? Category { get; set; }

        [MaxLength(20)]
        public string Source { get; set; } = "AI";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public InterviewSession? InterviewSession { get; set; }
    }
}
