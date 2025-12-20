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

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Resume> Resumes { get; set; } = null!;
        public DbSet<ResumeText> ResumeTexts { get; set; } = null!;
        public DbSet<ResumeFeedback> ResumeFeedbacks { get; set; } = null!;
        public DbSet<MatchedKeyword> MatchedKeywords { get; set; } = null!;
        public DbSet<InterviewSession> InterviewSessions { get; set; } = null!;
        public DbSet<Analytics> Analytics { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Entity<Resume>()
                .HasOne(r => r.User)
                .WithMany(u => u.Resumes)
                .HasForeignKey(r => r.UserId)
                .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);

            builder.Entity<ResumeText>()
                .HasOne(rt => rt.Resume)
                .WithOne(r => r.ResumeText)
                .HasForeignKey<ResumeText>(rt => rt.ResumeId)
                .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);

            builder.Entity<ResumeFeedback>()
                .HasOne(f => f.Resume)
                .WithMany(r => r.Feedbacks)
                .HasForeignKey(f => f.ResumeId)
                .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);

            builder.Entity<MatchedKeyword>()
                .HasOne(mk => mk.Resume)
                .WithMany(r => r.MatchedKeywords)
                .HasForeignKey(mk => mk.ResumeId)
                .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);

            builder.Entity<InterviewSession>()
                .HasOne(i => i.Resume)
                .WithMany(r => r.InterviewSessions)
                .HasForeignKey(i => i.ResumeId)
                .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);

            builder.Entity<Analytics>()
                .Property(a => a.Event)
                .HasMaxLength(200);
        }
    }
}
