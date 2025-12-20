using HireZ.Data;
using HireZ.DTOs.Resume;
using HireZ.Models;
using HireZ.Services.Background;
using Microsoft.EntityFrameworkCore;

namespace HireZ.Services
{
    public class ResumeService : IResumeService
    {
        private readonly ApplicationDbContext _db;
        private readonly IFileStorageService _fileStorage;
        private readonly ITextExtractionService _extractor;
        private readonly ResumeAnalysisQueue _queue;

        public ResumeService(ApplicationDbContext db, IFileStorageService fileStorage, ITextExtractionService extractor, ResumeAnalysisQueue queue)
        {
            _db = db;
            _fileStorage = fileStorage;
            _extractor = extractor;
            _queue = queue;
        }

        public async Task<int> UploadAsync(int userId, Stream fileStream, string fileName)
        {
            // 1. Save file
            var relative = await _fileStorage.SaveFileAsync(fileStream, fileName);

            // 2. Create Resume record
            var resume = new Resume
            {
                UserId = userId,
                FileName = fileName,
                FilePath = relative,
                Status = ResumeStatus.Uploaded
            };
            _db.Resumes.Add(resume);
            await _db.SaveChangesAsync();

            // 3. Extract text synchronously (fast path)
            var text = await _extractor.ExtractTextAsync(relative);
            if (!string.IsNullOrWhiteSpace(text))
            {
                _db.ResumeTexts.Add(new ResumeText { ResumeId = resume.Id, Text = text });
                resume.Status = ResumeStatus.TextExtracted;
                await _db.SaveChangesAsync();
            }

            // 4. Queue analysis
            await _queue.EnqueueAsync(resume.Id);

            return resume.Id;
        }

        public async Task QueueAnalysisAsync(int resumeId)
        {
            await _queue.EnqueueAsync(resumeId);
        }

        public async Task<ResumeDto?> GetResumeAsync(int resumeId)
        {
            var r = await _db.Resumes
                .Include(x => x.ResumeText)
                .Include(x => x.Feedbacks)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == resumeId);

            if (r == null) return null;

            return new ResumeDto
            {
                Id = r.Id,
                FileName = r.FileName,
                FilePath = r.FilePath,
                UploadedAt = r.UploadedAt,
                ExtractedText = r.ResumeText?.Text,
                Status = r.Status.ToString()
            };
        }

        public async Task<string?> GetExtractedTextAsync(int resumeId)
        {
            var t = await _db.ResumeTexts.AsNoTracking().FirstOrDefaultAsync(rt => rt.ResumeId == resumeId);
            return t?.Text;
        }
    }
}
