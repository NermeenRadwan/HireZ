using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HireZ.Data;
using HireZ.Utilities;
using Microsoft.EntityFrameworkCore;

namespace HireZ.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly ApplicationDbContext _db;
        private readonly IAiClient _aiClient;

        public InterviewService(ApplicationDbContext db, IAiClient aiClient)
        {
            _db = db;
            _aiClient = aiClient;
        }

        // Heuristic fallback generator (kept)
        public Task<List<string>> GenerateInterviewQuestionsAsync(string resumeText, string? jobDescription = null, int count = 8)
        {
            var resumeKeywords = TextProcessing.ExtractKeywords(resumeText);
            var jobKeywords = TextProcessing.ExtractKeywords(jobDescription ?? "");

            var common = resumeKeywords.Intersect(jobKeywords, StringComparer.OrdinalIgnoreCase).ToList();
            var resumeOnly = resumeKeywords.Except(common, StringComparer.OrdinalIgnoreCase).ToList();

            var questions = new List<string>();

            foreach (var kw in common.Take(count))
            {
                questions.Add($"Tell me about your experience with '{kw}'. What specific projects used this skill and what were the outcomes?");
            }

            foreach (var kw in resumeOnly.Take(Math.Max(0, (count / 2) - questions.Count)))
            {
                questions.Add($"Describe a situation where you applied '{kw}' and what you learned from it.");
            }

            var behavioral = new[]
            {
                "Tell me about a time you faced a significant technical challenge. How did you approach solving it?",
                "Give an example of when you had to work closely with a team to deliver on a deadline. What was your role?",
                "Describe an instance when you received constructive feedback. How did you act on it?",
                "How do you prioritize tasks when you have multiple high-priority items?"
            };

            foreach (var bq in behavioral)
            {
                if (questions.Count >= count) break;
                questions.Add(bq);
            }

            if (!string.IsNullOrWhiteSpace(jobDescription) && questions.Count < count)
            {
                var sentence = ExtractRepresentativeSentence(jobDescription);
                if (!string.IsNullOrEmpty(sentence))
                {
                    questions.Add($"Given the following job expectation: \"{sentence}\" — how would you approach this requirement in your first 30 days?");
                }
            }

            var generalTech = new[]
            {
                "Explain a recent project where you improved performance or scalability.",
                "How do you ensure code quality and maintainability in your projects?"
            };

            foreach (var gt in generalTech)
            {
                if (questions.Count >= count) break;
                questions.Add(gt);
            }

            return Task.FromResult(questions.Take(count).ToList());
        }

        public async Task<List<string>> GenerateInterviewQuestionsAsync(int resumeId, int? jobId = null, int count = 8)
        {
            var resume = await _db.Resumes.Include(r => r.ResumeText).FirstOrDefaultAsync(r => r.Id == resumeId);
            if (resume == null) throw new InvalidOperationException("Resume not found");

            // Ensure resumeText is a string
            var resumeText = resume.ResumeText?.Text ?? resume.ResumeText?.ToString() ?? string.Empty;

            string? jobDescription = null;
            if (jobId.HasValue)
            {
                var job = await _db.Jobs.FindAsync(jobId.Value);
                if (job != null) jobDescription = $"{job.Title} {job.Description} {job.Requirements}";
            }

            return await GenerateInterviewQuestionsAsync(resumeText, jobDescription, count);
        }

        /// <summary>
        /// Create interview session, ask AI to generate structured questions,
        /// persist them to InterviewQuestion table and return session id.
        /// Falls back to heuristic generator on failure.
        /// </summary>
        public async Task<int> CreateInterviewSessionAndGenerateAsync(int resumeId, int? jobId, int count = 8, string preferredSource = "ai")
        {
            var session = new Models.InterviewSession
            {
                ResumeId = resumeId,
                JobId = jobId,
                Status = "Queued",
                CreatedAt = DateTime.UtcNow
            };
            _db.InterviewSessions.Add(session);
            await _db.SaveChangesAsync();

            List<(string QuestionText, string Category, string Source)> questionsResult = new();

            if (!string.IsNullOrWhiteSpace(preferredSource) && preferredSource.ToLowerInvariant() == "ai")
            {
                try
                {
                    var resume = await _db.Resumes.Include(r => r.ResumeText).FirstOrDefaultAsync(r => r.Id == resumeId);
                    if (resume == null) throw new InvalidOperationException("Resume not found");

                    var resumeText = resume.ResumeText?.Text ?? resume.ResumeText?.ToString() ?? string.Empty;

                    string? jobDescription = null;
                    if (jobId.HasValue)
                    {
                        var job = await _db.Jobs.FindAsync(jobId.Value);
                        if (job != null) jobDescription = $"{job.Title} {job.Description} {job.Requirements}";
                    }

                    var prompt = AiPromptBuilder.BuildInterviewQuestionsPrompt(resumeText, jobDescription, count);
                    var aiResponse = await _aiClient.GenerateAsync(prompt) ?? string.Empty;

                    var parsed = ParseAiQuestions(aiResponse, count);
                    if (parsed.Count > 0)
                    {
                        questionsResult = parsed;
                    }
                    else
                    {
                        // fallback to heuristic
                        var fallback = await GenerateInterviewQuestionsAsync(resumeText, jobDescription, count);
                        questionsResult = fallback.Select(q => (q, "mixed", "Heuristic")).ToList();
                    }
                }
                catch
                {
                    var fallback = await GenerateInterviewQuestionsAsync(resumeId, jobId, count);
                    questionsResult = fallback.Select(q => (q, "mixed", "Heuristic")).ToList();
                }
            }
            else
            {
                var fallback = await GenerateInterviewQuestionsAsync(resumeId, jobId, count);
                questionsResult = fallback.Select(q => (q, "mixed", "Heuristic")).ToList();
            }

            // Persist top 'count' questions
            foreach (var q in questionsResult.Take(count))
            {
                var iq = new Models.InterviewQuestion
                {
                    InterviewSessionId = session.Id,
                    QuestionText = q.QuestionText,
                    Category = string.IsNullOrWhiteSpace(q.Category) ? null : q.Category,
                    Source = q.Source,
                    CreatedAt = DateTime.UtcNow
                };
                _db.InterviewQuestions.Add(iq);
            }

            session.Status = "Generated";
            await _db.SaveChangesAsync();

            return session.Id;
        }

        public async Task<Models.InterviewSession?> GetSessionAsync(int sessionId)
        {
            return await _db.InterviewSessions
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(s => s.Id == sessionId);
        }

        private List<(string QuestionText, string Category, string Source)> ParseAiQuestions(string aiResponse, int limit)
        {
            var list = new List<(string QuestionText, string Category, string Source)>();

            if (string.IsNullOrWhiteSpace(aiResponse)) return list;

            try
            {
                using var doc = JsonDocument.Parse(aiResponse);
                if (doc.RootElement.ValueKind != JsonValueKind.Array) return list;

                foreach (var el in doc.RootElement.EnumerateArray())
                {
                    var q = el.TryGetProperty("question", out var qElem) ? qElem.GetString() ?? "" : "";
                    var cat = el.TryGetProperty("category", out var cElem) ? cElem.GetString() : null;
                    if (!string.IsNullOrWhiteSpace(q))
                    {
                        list.Add((q.Trim(), string.IsNullOrWhiteSpace(cat) ? "mixed" : cat.Trim(), "AI"));
                    }
                    if (list.Count >= limit) break;
                }
            }
            catch
            {
                // parsing failed, return empty list as fallback caller will handle it
                return new List<(string, string, string)>();
            }

            return list;
        }

        private string? ExtractRepresentativeSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var sentences = System.Text.RegularExpressions.Regex.Split(text, @"(?<=[\.!\?])\s+");
            var ordered = sentences.OrderByDescending(s => s.Length);
            foreach (var s in ordered)
            {
                if (s.Length > 20 && s.Length < 300)
                {
                    return s.Trim();
                }
            }
            return sentences.FirstOrDefault()?.Trim();
        }
    }
}
