namespace HireZ.DTOs.Feedback
{
    public class ResumeFeedbackDto
    {
        public int ResumeId { get; set; }
        public double? AtsScore { get; set; }
        public string FeedbackJson { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
