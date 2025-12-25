namespace HireZ.DTOs
{
    public class CreateInterviewSessionRequest
    {
        public int ResumeId { get; set; }
        public int? JobId { get; set; }
        public int Count { get; set; } = 8; // number of questions
        // Optionally allow client to prefer "ai" or "heuristic"
        public string? PreferredSource { get; set; } = "ai";
    }
}
