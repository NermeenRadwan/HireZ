namespace HireZ.DTOs
{
    public class CreateFeedbackRequest
    {
        public string? CandidateName { get; set; }
        public string? InterviewType { get; set; }
        public int? Rating { get; set; }
        public string? TechnicalSkills { get; set; }
        public string? Communication { get; set; }
        public string? ProblemSolving { get; set; }
        public string? CulturalFit { get; set; }
        public string? Comments { get; set; }
        public string? Recommendation { get; set; }
    }
}

