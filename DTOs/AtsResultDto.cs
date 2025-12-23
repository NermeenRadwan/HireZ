using System.Collections.Generic;

namespace HireZ.DTOs
{
    public class AtsResultDto
    {
        public int JobId { get; set; }
        public int ResumeId { get; set; }
        public int AtsScore { get; set; } // 0-100
        public List<string> MatchedKeywords { get; set; } = new();
        public List<string> MissingKeywords { get; set; } = new();
        public string Summary { get; set; } = "";
    }
}
