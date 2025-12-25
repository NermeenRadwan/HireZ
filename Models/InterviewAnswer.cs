using System;
using System.ComponentModel.DataAnnotations;

namespace HireZ.Models
{
    public class InterviewAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InterviewQuestionId { get; set; }

        public string? AnswerText { get; set; }

        // optional numeric score (0-100)
        public int? Score { get; set; }

        // optional AI feedback text
        public string? Feedback { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
