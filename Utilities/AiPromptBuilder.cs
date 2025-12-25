using System.Text;

namespace HireZ.Utilities
{
    public static class AiPromptBuilder
    {
        /// <summary>
        /// Builds a prompt that instructs the AI to return a JSON array of objects:
        /// [{ "question": "...", "category": "technical" }]
        /// </summary>
        public static string BuildInterviewQuestionsPrompt(string resumeText, string? jobDescription, int count)
        {
            var sb = new StringBuilder();
            sb.AppendLine("You are an expert technical interviewer. Generate a JSON array of interview question objects.");
            sb.AppendLine($"Return exactly a JSON array (no explanation text). Each object should contain:");
            sb.AppendLine("{ \"question\": \"...\", \"category\": \"technical|behavioral|scenario\" }");
            sb.AppendLine();
            sb.AppendLine($"Requirements:");
            sb.AppendLine($"- Produce {count} well-targeted interview questions.");
            sb.AppendLine($"- Use resume content to create targeted technical questions.");
            sb.AppendLine($"- Use job description to craft scenario-based and role-specific questions (if provided).");
            sb.AppendLine($"- Prefer concise yet specific questions suitable for a 30-60 minute interview.");
            sb.AppendLine();
            sb.AppendLine("Resume:");
            sb.AppendLine(resumeText.Length > 10000 ? resumeText.Substring(0, 10000) : resumeText);
            if (!string.IsNullOrWhiteSpace(jobDescription))
            {
                sb.AppendLine();
                sb.AppendLine("Job Description:");
                sb.AppendLine(jobDescription.Length > 5000 ? jobDescription.Substring(0, 5000) : jobDescription);
            }
            sb.AppendLine();
            sb.AppendLine("Now output the JSON array only.");
            return sb.ToString();
        }
    }
}
