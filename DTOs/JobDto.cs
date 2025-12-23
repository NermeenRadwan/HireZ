using System;

namespace HireZ.DTOs
{
    public class JobDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Requirements { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
