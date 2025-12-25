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

        // Core domain sets (keep these names consistent with your models)
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

        // other DbSets remain...

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // InterviewSession -> InterviewQuestion (1:n)
            modelBuilder.Entity<InterviewSession>()
                .HasOne(s => s.Resume)
                .WithMany(r => r.InterviewSessions)  // uses inverse property on Resume
                .HasForeignKey(s => s.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            // InterviewSession -> Resume (many sessions per resume). Use WithMany() to avoid requiring inverse navigation property on Resume.
            modelBuilder.Entity<InterviewSession>()
                .HasOne<Resume>()
                .WithMany() // do not require Resume.InterviewSessions
                .HasForeignKey(s => s.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            // MatchedKeyword minimal mapping (if needed)
            modelBuilder.Entity<MatchedKeyword>()
                .HasKey(mk => mk.Id);

            // ResumeFeedback mapping (if needed)
            modelBuilder.Entity<ResumeFeedback>()
                .HasKey(rf => rf.Id);

            // Ensure unique constraints / indexes you rely on are defined, e.g. unique email on User if desired:
            // modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // Additional configuration here as required...
        }
    }
}
