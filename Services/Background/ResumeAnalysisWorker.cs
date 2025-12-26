using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using HireZ.Data;
using HireZ.Models;

namespace HireZ.Services.Background
{
    public class ResumeAnalysisWorker : BackgroundService
    {
        private readonly ILogger<ResumeAnalysisWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ResumeAnalysisQueue _queue;

        public ResumeAnalysisWorker(IServiceProvider serviceProvider, ResumeAnalysisQueue queue, ILogger<ResumeAnalysisWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ResumeAnalysisWorker started.");

            await foreach (var resumeId in _queue.DequeueAllAsync(stoppingToken))
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var aiService = scope.ServiceProvider.GetRequiredService<IAiService>(); // we'll add interface below
                    var extractService = scope.ServiceProvider.GetRequiredService<ITextExtractionService>();

                    var resume = await db.Resumes.Include(r => r.ResumeText).FirstOrDefaultAsync(r => r.Id == resumeId, stoppingToken);
                    if (resume == null)
                    {
                        _logger.LogWarning("Resume {id} not found.", resumeId);
                        continue;
                    }

                    // Ensure text exists
                    if (string.IsNullOrWhiteSpace(resume.ResumeText?.Text))
                    {
                        var text = await extractService.ExtractTextAsync(resume.FilePath, stoppingToken);
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            resume.ResumeText ??= new ResumeText { ResumeId = resume.Id };
                            resume.ResumeText.Text = text;
                            resume.Status = ResumeStatus.TextExtracted;
                            await db.SaveChangesAsync(stoppingToken);
                        }
                    }

                    // Call AI service to analyze and return structured JSON
                    resume.Status = ResumeStatus.AnalysisQueued;
                    await db.SaveChangesAsync(stoppingToken);

                    var analysis = await aiService.AnalyzeResumeAsync(resume.Id, resume.ResumeText?.Text ?? string.Empty, stoppingToken);

                    // Save feedback
                    db.ResumeFeedbacks.Add(new ResumeFeedback
                    {
                        ResumeId = resume.Id,
                        Type = FeedbackType.AiSummary,
                        FeedbackJson = analysis.FeedbackJson,
                        AtsScore = analysis.AtsScore
                    });

                    resume.Status = ResumeStatus.AnalysisCompleted;
                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing resume {id}", resumeId);
                    // attempt to mark resume as failed
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var r = await db.Resumes.FindAsync(new object[] { resumeId }, stoppingToken);
                        if (r != null)
                        {
                            r.Status = ResumeStatus.AnalysisFailed;
                            await db.SaveChangesAsync(stoppingToken);
                        }
                    }
                    catch { /* swallow */ }
                }
            }

            _logger.LogInformation("ResumeAnalysisWorker stopping.");
        }
    }
}
