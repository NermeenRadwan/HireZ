using System.Text.Json;

namespace HireZ.Services
{
    // TODO: replace with real OpenAI integration
    public class OpenAiServiceStub : IAiService
    {
        private readonly ILogger<OpenAiServiceStub> _logger;
        private readonly IConfiguration _config;

        public OpenAiServiceStub(ILogger<OpenAiServiceStub> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public Task<AiAnalysisResult> AnalyzeResumeAsync(int resumeId, string resumeText, CancellationToken cancellation = default)
        {
            // Simple fake analysis: count keywords and estimate an ATS score
            var keywords = new[] { "C#", ".NET", "SQL", "JavaScript", "Python", "Azure" };
            var present = keywords.Count(k => resumeText?.Contains(k, StringComparison.OrdinalIgnoreCase) == true);
            var atsScore = Math.Min(100, 40 + present * 10);

            var feedback = new
            {
                summary = "This is a placeholder AI summary. Replace OpenAiServiceStub with a real implementation.",
                keywordHits = present,
                recommendedImprovements = new[] { "Add achievements with numbers", "Use active verbs" }
            };
            var json = JsonSerializer.Serialize(feedback);

            return Task.FromResult(new AiAnalysisResult(json, atsScore));
        }
    }
}
