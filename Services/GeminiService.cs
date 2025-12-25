using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HireZ.Services
{
    public class GeminiService : IAiService
    {
        private readonly HttpClient _http;
        private readonly ILogger<GeminiService> _logger;
        private readonly string _apiKey;
        private readonly string _model;

        public GeminiService(
            HttpClient http,
            IConfiguration config,
            ILogger<GeminiService> logger)
        {
            _http = http;
            _logger = logger;

            _apiKey = config.GetValue<string>("Gemini:ApiKey")
                      ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY")
                      ?? throw new InvalidOperationException(
                          "Gemini API key is not configured. Set GEMINI_API_KEY or Gemini:ApiKey.");

            _model = config.GetValue<string>("Gemini:Model") ?? "gemini-1.5-flash";
        }

        public async Task<AiAnalysisResult> AnalyzeResumeAsync(
            int resumeId,
            string resumeText,
            CancellationToken cancellation = default)
        {
            if (string.IsNullOrWhiteSpace(resumeText))
            {
                return new AiAnalysisResult(
                    JsonSerializer.Serialize(new
                    {
                        summary = "",
                        ats_score = 0,
                        recommended_improvements = Array.Empty<string>()
                    }),
                    0);
            }

            // ✅ USE EXISTING PROMPT BUILDER
            var prompt = AiPromptBuilder.BuildResumeAnalysisPrompt(resumeText);

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

            try
            {
                var response = await _http.PostAsync(url, content, cancellation);
                var responseText = await response.Content.ReadAsStringAsync(cancellation);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "Gemini API error for resume {ResumeId}: {Response}",
                        resumeId,
                        responseText);

                    return new AiAnalysisResult(responseText, null);
                }

                using var doc = JsonDocument.Parse(responseText);

                var generatedText =
                    doc.RootElement
                       .GetProperty("candidates")[0]
                       .GetProperty("content")
                       .GetProperty("parts")[0]
                       .GetProperty("text")
                       .GetString();

                if (string.IsNullOrWhiteSpace(generatedText))
                {
                    return new AiAnalysisResult("{}", null);
                }

                // Try parsing ATS score from strict JSON
                double? atsScore = null;

                try
                {
                    var parsed = JsonSerializer.Deserialize<JsonElement>(generatedText);

                    if (parsed.ValueKind == JsonValueKind.Object &&
                        parsed.TryGetProperty("ats_score", out var scoreProp))
                    {
                        if (scoreProp.TryGetDouble(out var score))
                            atsScore = score;
                    }

                    var normalized = JsonSerializer.Serialize(parsed);
                    return new AiAnalysisResult(normalized, atsScore);
                }
                catch (JsonException)
                {
                    // Model violated JSON contract — store raw output
                    _logger.LogWarning(
                        "Gemini output was not valid JSON for resume {ResumeId}",
                        resumeId);

                    return new AiAnalysisResult(generatedText, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Gemini request failed for resume {ResumeId}",
                    resumeId);

                return new AiAnalysisResult("", null);
            }
        }

        public Task<string> GenerateAsync(object prompt)
        {
            throw new NotImplementedException();
        }
    }
}
