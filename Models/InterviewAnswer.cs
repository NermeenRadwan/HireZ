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

        public int? Score { get; set; }

        public string? Feedback { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
