using System;

namespace HireZ.Models
{
    public class Analytics
    {
        public int Id { get; set; }
        public string Event { get; set; } = null!;
        public string DataJson { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
