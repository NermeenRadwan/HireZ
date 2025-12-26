using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HireZ.Models
{
    public class InterviewSession
    {
        [Key]
        public int Id { get; set; }

        // FK to Resume - make sure this is same type as Resume.Id (int)
        [Required]
        public int ResumeId { get; set; }

        // Navigation property - ensure EF sees this
        [ForeignKey(nameof(ResumeId))]
        public virtual Resume? Resume { get; set; }

        // Optional job reference
        public int? JobId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Queued";

        public virtual ICollection<InterviewQuestion> Questions { get; set; } = new List<InterviewQuestion>();
    }
}
