using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using HireZ.DTOs;
using HireZ.Models;
using HireZ.Utilities;
using HireZ.Data; // ApplicationDbContext namespace - adjust if different

namespace HireZ.Services
{
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext _db;
        public JobService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<JobDto> CreateJobAsync(CreateJobRequest request)
        {
            var job = new Job
            {
                Title = request.Title,
                Description = request.Description,
                Requirements = request.Requirements ?? ""
            };
            _db.Jobs.Add(job);
            await _db.SaveChangesAsync();

            return new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Requirements = job.Requirements,
                CreatedAt = job.CreatedAt
            };
        }

        public async Task<JobDto?> GetJobAsync(int jobId)
        {
            var j = await _db.Jobs.FindAsync(jobId);
            if (j == null) return null;
            return new JobDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                Requirements = j.Requirements,
                CreatedAt = j.CreatedAt
            };
        }

        public async Task<AtsResultDto> MatchResumeToJobAsync(int jobId, int resumeId)
        {
            var job = await _db.Jobs.FindAsync(jobId)
                      ?? throw new InvalidOperationException("Job not found");

            var resume = await _db.Resumes
                .Include(r => r.ResumeText)
                .FirstOrDefaultAsync(r => r.Id == resumeId);

            if (resume == null) throw new InvalidOperationException("Resume not found");

            // Use the extracted text if available, otherwise empty string
            var resumeText = resume.ResumeText?.Text ?? "";

            var jobText = $"{job.Title} {job.Requirements} {job.Description}";

            // extract keywords
            var jobKeywords = TextProcessing.ExtractKeywords(jobText);
            var resumeKeywords = TextProcessing.ExtractKeywords(resumeText);

            var totalKeywords = jobKeywords.Count == 0 ? 1 : jobKeywords.Count;

            var matched = jobKeywords.Intersect(resumeKeywords, StringComparer.OrdinalIgnoreCase).ToList();
            var missing = jobKeywords.Except(resumeKeywords, StringComparer.OrdinalIgnoreCase).ToList();

            var score = (int)Math.Round(100.0 * matched.Count / totalKeywords);

            // Persist matched keywords (only assign fields we know exist: ResumeId, Keyword)
            // If MatchedKeyword entity has other fields (e.g., CreatedAt) they'll use defaults
            if (_db.Set<MatchedKeyword>() != null)
            {
                foreach (var kw in matched)
                {
                    var mk = new MatchedKeyword
                    {
                        ResumeId = resume.Id,
                        Keyword = kw
                        // Note: do not set JobId/MatchedAt here because those properties don't exist in your model
                    };
                    _db.MatchedKeywords.Add(mk);
                }
                await _db.SaveChangesAsync();
            }

            // Persist a minimal ResumeFeedback entry if the entity exists in your model.
            // We only set fields that are commonly present: ResumeId and CreatedAt.
            if (_db.Set<ResumeFeedback>() != null)
            {
                var feedback = new ResumeFeedback
                {
                    ResumeId = resume.Id,
                    CreatedAt = DateTime.UtcNow
                    // Do not set FeedbackText or Source because those properties were not present in your model.
                    // If your ResumeFeedback requires a non-null field, add it here (adjust to your model).
                };
                _db.ResumeFeedbacks.Add(feedback);
                await _db.SaveChangesAsync();
            }

            return new AtsResultDto
            {
                JobId = job.Id,
                ResumeId = resume.Id,
                AtsScore = score,
                MatchedKeywords = matched,
                MissingKeywords = missing,
                Summary = $"Matched {matched.Count} of {jobKeywords.Count} required keywords."
            };
        }
    }
}
