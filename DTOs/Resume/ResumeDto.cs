using System;

namespace HireZ.DTOs.Resume
{
    public class ResumeDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
        public string? ExtractedText { get; set; }
        public string Status { get; set; } = null!;
    }
}
