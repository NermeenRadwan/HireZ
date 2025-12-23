using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HireZ.Data; // ApplicationDbContext
using HireZ.Utilities;
using Microsoft.EntityFrameworkCore;

namespace HireZ.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly ApplicationDbContext _db;
        public InterviewService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<string>> GenerateInterviewQuestionsAsync(int resumeId, int? jobId = null, int count = 8)
        {
            var resume = await _db.Resumes.Include(r => r.ResumeText).FirstOrDefaultAsync(r => r.Id == resumeId);
            if (resume == null) throw new InvalidOperationException("Resume not found");

            string resumeText = resume.ResumeText?.Text ?? "";

            string? jobDescription = null;
            if (jobId.HasValue)
            {
                var job = await _db.Jobs.FindAsync(jobId.Value);
                if (job != null) jobDescription = $"{job.Title} {job.Description} {job.Requirements}";
            }

            return await GenerateInterviewQuestionsAsync(resumeText, jobDescription, count);
        }

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

        private string? ExtractRepresentativeSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var sentences = Regex.Split(text, @"(?<=[\.!\?])\s+");
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
