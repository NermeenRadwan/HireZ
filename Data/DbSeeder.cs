using System.Linq;
using HireZ.Models;
using HireZ.Utilities;

namespace HireZ.Data
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            if (db.Users.Any()) return; // already seeded

            var admin = new User
            {
                Email = "admin@example.com",
                PasswordHash = PasswordHasher.Hash("Admin@12345"),
                Role = "Admin"
            };
            db.Users.Add(admin);
            db.SaveChanges();

            var resume = new Resume
            {
                UserId = admin.Id,
                FileName = "sample_resume.pdf",
                FilePath = "uploads/sample_resume.pdf"
            };
            db.Resumes.Add(resume);
            db.SaveChanges();

            db.ResumeTexts.Add(new ResumeText
            {
                ResumeId = resume.Id,
                Text = "Sample extracted text from resume."
            });

            db.ResumeFeedbacks.Add(new ResumeFeedback
            {
                ResumeId = resume.Id,
                FeedbackJson = "{\"summary\":\"This is a sample feedback\"}",
                AtsScore = 72.5
            });

            db.MatchedKeywords.Add(new MatchedKeyword
            {
                ResumeId = resume.Id,
                Keyword = "C#",
                IsPresent = true
            });

            db.SaveChanges();
        }
    }
}
