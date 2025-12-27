using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using HireZ.DTOs;
using HireZ.Models;
using HireZ.Utilities;
using HireZ.Data;

namespace HireZ.Services
{
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext _db;
        public JobService(ApplicationDbContext db) { _db = db; }

        public async Task<JobDto> CreateJobAsync(CreateJobRequest request)
        {
            var job = new Job { Title = request.Title, Description = request.Description, Requirements = request.Requirements ?? "" };
            _db.Jobs.Add(job);
            await _db.SaveChangesAsync();
            return new JobDto { Id = job.Id, Title = job.Title, Description = job.Description, Requirements = job.Requirements, CreatedAt = job.CreatedAt };
        }

        public async Task<JobDto?> GetJobAsync(int jobId)
        {
            var j = await _db.Jobs.FindAsync(jobId);
            if (j == null) return null;
            return new JobDto { Id = j.Id, Title = j.Title, Description = j.Description, Requirements = j.Requirements, CreatedAt = j.CreatedAt };
        }

        public async Task<AtsResultDto> MatchResumeToJobAsync(int jobId, int resumeId)
        {
            var job = await _db.Jobs.FindAsync(jobId) ?? throw new InvalidOperationException("Job not found");
            var resume = await _db.Resumes.Include(r => r.ResumeText).FirstOrDefaultAsync(r => r.Id == resumeId) ?? throw new InvalidOperationException("Resume not found");

            var resumeText = resume.ResumeText?.Text ?? "";
            var jobText = $"{job.Title} {job.Requirements} {job.Description}";

            var jobKeywords = TextProcessing.ExtractKeywords(jobText);
            var resumeKeywords = TextProcessing.ExtractKeywords(resumeText);

            var matched = jobKeywords.Intersect(resumeKeywords, StringComparer.OrdinalIgnoreCase).ToList();
            var missing = jobKeywords.Except(resumeKeywords, StringComparer.OrdinalIgnoreCase).ToList();

            var totalKeywords = jobKeywords.Count == 0 ? 1 : jobKeywords.Count;
            var score = (int)Math.Round(100.0 * matched.Count / totalKeywords);

            // Persist matched keywords (safe fields only)
            try
            {
                foreach (var kw in matched)
                {
                    _db.MatchedKeywords.Add(new MatchedKeyword { ResumeId = resume.Id, Keyword = kw });
                }
                await _db.SaveChangesAsync();
            }
            catch { /* ignore persistence fail for optional table */ }

            // Minimal resume feedback persistence (if exists)
            try
            {
                _db.ResumeFeedbacks.Add(new ResumeFeedback { ResumeId = resume.Id, CreatedAt = DateTime.UtcNow });
                await _db.SaveChangesAsync();
            }
            catch { /* ignore */ }

            return new AtsResultDto { JobId = job.Id, ResumeId = resume.Id, AtsScore = score, MatchedKeywords = matched, MissingKeywords = missing, Summary = $"Matched {matched.Count} of {jobKeywords.Count} required keywords." };
        }
    }
}
