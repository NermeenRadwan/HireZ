namespace HireZ.Services
{
    public record AiAnalysisResult(string FeedbackJson, double? AtsScore);

    public interface IAiService
    {
        Task<AiAnalysisResult> AnalyzeResumeAsync(int resumeId, string resumeText, CancellationToken cancellation = default);
    }
}
