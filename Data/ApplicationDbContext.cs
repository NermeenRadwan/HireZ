using Microsoft.EntityFrameworkCore;
using HireZ.Models;

namespace HireZ.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Existing / core sets
        public DbSet<User> Users { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<ResumeText> ResumeTexts { get; set; }
        public DbSet<MatchedKeyword> MatchedKeywords { get; set; }
        public DbSet<ResumeFeedback> ResumeFeedbacks { get; set; }

        // Jobs & ATS
        public DbSet<Job> Jobs { get; set; }

        // Interview entities
        public DbSet<InterviewSession> InterviewSessions { get; set; }
        public DbSet<InterviewQuestion> InterviewQuestions { get; set; }
        public DbSet<InterviewAnswer> InterviewAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // InterviewSession -> InterviewQuestion (1:n)
            modelBuilder.Entity<InterviewSession>()
                .HasMany(s => s.Questions)
                .WithOne(q => q.InterviewSession)
                .HasForeignKey(q => q.InterviewSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // InterviewSession -> Resume (explicit mapping)
            modelBuilder.Entity<InterviewSession>()
                .HasOne(s => s.Resume)
                .WithMany(r => r.InterviewSessions) // ensure Resume model has InterviewSessions collection; if not, use .WithMany()
                .HasForeignKey(s => s.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Additional model config can go here...
        }
    }
}
