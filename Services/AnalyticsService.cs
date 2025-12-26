using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HireZ.Data;
using HireZ.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HireZ.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _db;
        public AnalyticsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<AnalyticsOverviewDto> GetOverviewAsync()
        {
            // Load required datasets into memory (defensive - avoids EF translation issues if column names differ)
            List<object> usersList = new();
            List<object> resumesList = new();
            List<object> resumeTextsList = new();
            List<object> feedbacksList = new();
            List<object> interviewsList = new();

            try
            {
                try { usersList = (await _db.Users.ToListAsync()).Cast<object>().ToList(); } catch { usersList = new(); }
            }
            catch { usersList = new(); }

            try
            {
                resumesList = (await _db.Resumes
                    .Include(r => r.ResumeText)
                    .ToListAsync()).Cast<object>().ToList();
            }
            catch
            {
                resumesList = new();
            }

            try
            {
                resumeTextsList = (await _db.ResumeTexts.ToListAsync()).Cast<object>().ToList();
            }
            catch
            {
                resumeTextsList = new();
            }

            try
            {
                feedbacksList = (await _db.ResumeFeedbacks.ToListAsync()).Cast<object>().ToList();
            }
            catch
            {
                feedbacksList = new();
            }

            try
            {
                interviewsList = (await _db.InterviewSessions.ToListAsync()).Cast<object>().ToList();
            }
            catch
            {
                interviewsList = new();
            }

            int totalUsers = usersList.Count;
            int resumesUploaded = resumesList.Count;

            // resumes with text extracted: look for ResumeText navigation or a textual column on Resume
            int resumesTextExtracted = 0;
            try
            {
                resumesTextExtracted = resumesList.Count(r =>
                {
                    var rt = TryGetPropertyValue(r, "ResumeText");
                    if (rt != null)
                    {
                        var txt = TryGetPropertyValue(rt, "Text") as string;
                        return !string.IsNullOrWhiteSpace(txt);
                    }

                    var alt = TryGetPropertyValue(r, "Text") as string
                              ?? TryGetPropertyValue(r, "ExtractedText") as string
                              ?? TryGetPropertyValue(r, "Content") as string;
                    return !string.IsNullOrWhiteSpace(alt);
                });
            }
            catch
            {
                resumesTextExtracted = 0;
            }

            // resumes analyzed = count of distinct ResumeId in feedbacks
            int resumesAnalyzed = 0;
            try
            {
                var ids = new HashSet<int>();
                foreach (var f in feedbacksList)
                {
                    var ridObj = TryGetPropertyValue(f, "ResumeId") ?? TryGetPropertyValue(f, "resumeId");
                    if (ridObj != null && IntTryConvert(ridObj, out int rid))
                        ids.Add(rid);
                }
                resumesAnalyzed = ids.Count;
            }
            catch
            {
                resumesAnalyzed = 0;
            }

            int interviewSessions = interviewsList.Count;

            // Average time to analysis (in days) - compute from resume upload -> earliest feedback for that resume
            double avgTimeToAnalysisDays = 0;
            try
            {
                // Build a map of resumeId -> uploadDate (DateTime?)
                var resumeUploadMap = new Dictionary<int, DateTime?>();
                foreach (var r in resumesList)
                {
                    var idObj = TryGetPropertyValue(r, "Id") ?? TryGetPropertyValue(r, "ResumeId");
                    if (!IntTryConvert(idObj, out int rid)) continue;

                    DateTime? uploadDt = NullDateTime();

                    // Try common property names for upload/created
                    uploadDt = TryGetDateTimeProperty(r, new[] { "UploadedAt", "UploadedOn", "CreatedAt", "Created", "UploadDate", "DateUploaded" })
                               ?? TryGetDateTimeFromResumeText(r);

                    resumeUploadMap[rid] = uploadDt;
                }

                // Collect deltas
                var deltas = new List<double>();
                foreach (var f in feedbacksList)
                {
                    var ridObj = TryGetPropertyValue(f, "ResumeId") ?? TryGetPropertyValue(f, "resumeId");
                    if (!IntTryConvert(ridObj, out int rid)) continue;

                    var feedbackDt = TryGetDateTimeProperty(f, new[] { "CreatedAt", "Created", "Timestamp", "Date" });
                    if (feedbackDt == null) continue;

                    if (!resumeUploadMap.TryGetValue(rid, out var uploadDt) || uploadDt == null) continue;

                    var ts = (feedbackDt.Value - uploadDt.Value).TotalDays;
                    if (!double.IsNaN(ts) && !double.IsInfinity(ts))
                        deltas.Add(ts);
                }

                if (deltas.Any()) avgTimeToAnalysisDays = deltas.Average();
            }
            catch
            {
                avgTimeToAnalysisDays = 0;
            }

            // Pipeline counts
            var pipeline = new List<PipelineStageCount>
            {
                new PipelineStageCount { Stage = "Uploaded", Count = resumesUploaded },
                new PipelineStageCount { Stage = "TextExtracted", Count = resumesTextExtracted },
                new PipelineStageCount { Stage = "Analyzed", Count = resumesAnalyzed },
                new PipelineStageCount { Stage = "InterviewSessions", Count = interviewSessions }
            };

            return new AnalyticsOverviewDto
            {
                TotalUsers = totalUsers,
                ResumesUploaded = resumesUploaded,
                ResumesWithTextExtracted = resumesTextExtracted,
                ResumesAnalyzed = resumesAnalyzed,
                InterviewSessions = interviewSessions,
                AvgTimeToAnalysisDays = Math.Round(avgTimeToAnalysisDays, 2),
                Pipeline = pipeline
            };
        }

        public async Task<List<TrendPointDto>> GetResumeTrendsAsync(int days)
        {
            if (days <= 0) days = 30;
            var end = DateTime.UtcNow.Date;
            var start = end.AddDays(-days + 1);

            // Materialize lists
            List<object> resumesList;
            List<object> feedbacksList;
            try
            {
                resumesList = (await _db.Resumes.Include(r => r.ResumeText).ToListAsync()).Cast<object>().ToList();
            }
            catch
            {
                resumesList = new List<object>();
            }
            try
            {
                feedbacksList = (await _db.ResumeFeedbacks.ToListAsync()).Cast<object>().ToList();
            }
            catch
            {
                feedbacksList = new List<object>();
            }

            var result = new List<TrendPointDto>();
            for (int i = 0; i < days; i++)
            {
                result.Add(new TrendPointDto { Date = start.AddDays(i), UploadedCount = 0, AnalyzedCount = 0 });
            }

            // Build upload date map
            foreach (var r in resumesList)
            {
                var idObj = TryGetPropertyValue(r, "Id") ?? TryGetPropertyValue(r, "ResumeId");
                if (!IntTryConvert(idObj, out int rid)) continue;

                var uploadDt = TryGetDateTimeProperty(r, new[] { "UploadedAt", "UploadedOn", "CreatedAt", "Created", "UploadDate", "DateUploaded" })
                               ?? TryGetDateTimeFromResumeText(r);

                if (uploadDt == null) continue;
                var d = uploadDt.Value.Date;
                if (d >= start && d <= end)
                {
                    var p = result.FirstOrDefault(x => x.Date.Date == d);
                    if (p != null) p.UploadedCount++;
                }
            }

            // analyses by feedback createdAt
            foreach (var f in feedbacksList)
            {
                var created = TryGetDateTimeProperty(f, new[] { "CreatedAt", "Created", "Timestamp", "Date" });
                if (created == null) continue;
                var d = created.Value.Date;
                if (d >= start && d <= end)
                {
                    var p = result.FirstOrDefault(x => x.Date.Date == d);
                    if (p != null) p.AnalyzedCount++;
                }
            }

            return result;
        }

        #region Helpers

        private static bool IntTryConvert(object? obj, out int value)
        {
            value = 0;
            if (obj == null) return false;
            if (obj is int i) { value = i; return true; }
            if (obj is long l) { value = (int)l; return true; }
            if (int.TryParse(obj.ToString(), out int parsed)) { value = parsed; return true; }
            return false;
        }

        private static DateTime? TryGetDateTimeProperty(object obj, string[] propNames)
        {
            foreach (var name in propNames)
            {
                var p = obj.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (p != null)
                {
                    var val = p.GetValue(obj);
                    if (val is DateTime dt) return dt;

                    // Fallback: try to parse string or convert other types to DateTime
                    if (val != null)
                    {
                        if (val is DateTimeOffset dto) return dto.UtcDateTime;
                        if (val is string s && DateTime.TryParse(s, out var parsed)) return parsed;
                        // last attempt: try ToString and parse
                        if (DateTime.TryParse(val.ToString(), out var parsed2)) return parsed2;
                    }
                }
            }
            return null;
        }

        private static object? TryGetPropertyValue(object obj, string propName)
        {
            var p = obj.GetType().GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (p == null) return null;
            return p.GetValue(obj);
        }

        private static DateTime? TryGetDateTimeFromResumeText(object resumeObj)
        {
            // Try resume.ResumeText.CreatedAt or Resume.ResumeText.Created
            var rt = TryGetPropertyValue(resumeObj, "ResumeText");
            if (rt == null) return null;
            return TryGetDateTimeProperty(rt, new[] { "CreatedAt", "Created", "Timestamp", "Date" });
        }

        private static DateTime? NullDateTime() => null;

        #endregion
    }
}
