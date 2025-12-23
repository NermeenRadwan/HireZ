using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HireZ.Models
{
    public class Job
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional: requirements as free text (or a normalized list in future)
        public string Requirements { get; set; } = "";
    }
}
