using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HireZ.Services
{
    /// <summary>
    /// Real OpenAI integration using the Responses API.
    /// Returns AiAnalysisResult(FeedbackJson, AtsScore).
    /// </summary>
    public class OpenAiService : IAiService
    {
        private readonly HttpClient _http;
        private readonly ILogger<OpenAiService> _logger;
        private readonly string _model;

        public OpenAiService(HttpClient http, IConfiguration config, ILogger<OpenAiService> logger)
        {
            _http = http;
            _logger = logger;
            // model can be configured in appsettings: OpenAI:Model
            _model = config.GetValue<string>("OpenAI:Model") ?? config.GetValue<string>("OpenAI:ModelName") ?? "gpt-4o";
        }

        public async Task<AiAnalysisResult> AnalyzeResumeAsync(int resumeId, string resumeText, CancellationToken cancellation = default)
        {
            if (string.IsNullOrWhiteSpace(resumeText))
            {
                var emptyJson = JsonSerializer.Serialize(new { summary = "", keywordHits = 0, recommendedImprovements = Array.Empty<string>() });
                return new AiAnalysisResult(emptyJson, 0);
            }

            var prompt = AiPromptBuilder.BuildResumeAnalysisPrompt(resumeText);

            var requestBody = new
            {
                model = _model,
                input = prompt,
                temperature = 0.0
            };

            string requestJson = JsonSerializer.Serialize(requestBody);
            using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            HttpResponseMessage resp;
            try
            {
                resp = await _http.PostAsync("/v1/responses", content, cancellation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OpenAI request failed for resume {id}", resumeId);
                throw;
            }

            var respText = await resp.Content.ReadAsStringAsync(cancellation);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("OpenAI API error {status}: {body}", resp.StatusCode, respText);
                return new AiAnalysisResult(respText, null);
            }

            try
            {
                // Parse response JSON (JsonNode is NOT IDisposable so do NOT use 'using')
                var doc = JsonNode.Parse(respText);
                string aggregatedText = ExtractAggregatedTextFromResponsesApi(doc);

                // Attempt to parse aggregatedText as JSON (we asked the model to return strict JSON)
                try
                {
                    var parsed = JsonSerializer.Deserialize<JsonElement>(aggregatedText);

                    double? ats = null;
                    if (parsed.ValueKind == JsonValueKind.Object && parsed.TryGetProperty("ats_score", out var scoreElem))
                    {
                        if (scoreElem.ValueKind == JsonValueKind.Number && scoreElem.TryGetDouble(out var d)) ats = d;
                        else if (scoreElem.ValueKind == JsonValueKind.String && double.TryParse(scoreElem.GetString(), out var d2)) ats = d2;
                    }

                    var normalized = JsonSerializer.Serialize(parsed, new JsonSerializerOptions { WriteIndented = false });
                    return new AiAnalysisResult(normalized, ats);
                }
                catch (JsonException)
                {
                    _logger.LogWarning("OpenAI response not parseable as strict JSON for resume {id}. Saving raw output.", resumeId);
                    return new AiAnalysisResult(respText, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process OpenAI response for resume {id}. Returning raw body.", resumeId);
                return new AiAnalysisResult(respText, null);
            }
        }

        /// <summary>
        /// Walks the Responses API JSON and attempts to extract an aggregated textual output.
        /// It prefers "output_text" property, then tries to concatenate text fields inside output[]->content[].
        /// </summary>
        private static string ExtractAggregatedTextFromResponsesApi(JsonNode? root)
        {
            if (root == null) return string.Empty;

            if (root["output_text"] != null)
            {
                return root["output_text"]!.GetValue<string>() ?? string.Empty;
            }

            var sb = new StringBuilder();
            var output = root["output"];
            if (output is JsonArray arr)
            {
                foreach (var item in arr)
                {
                    if (item == null) continue;
                    var content = item["content"];
                    if (content is JsonArray contentArr)
                    {
                        foreach (var c in contentArr)
                        {
                            if (c == null) continue;
                            if (c["text"] != null)
                            {
                                sb.AppendLine(c["text"]!.GetValue<string>());
                                continue;
                            }
                            if (c["type"] != null && c["type"]!.GetValue<string>() == "output_text" && c["text"] != null)
                            {
                                sb.AppendLine(c["text"]!.GetValue<string>());
                                continue;
                            }
                            if (c is JsonValue val && val.GetValue<string>() is string s)
                            {
                                sb.AppendLine(s);
                            }
                        }
                        continue;
                    }

                    if (item["text"] != null)
                    {
                        sb.AppendLine(item["text"]!.GetValue<string>());
                        continue;
                    }
                }
            }

            var result = sb.ToString().Trim();
            return result;
        }
    }
}
