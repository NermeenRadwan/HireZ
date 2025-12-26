
namespace HireZ.Services
{
    public static class AiPromptBuilder
    {
        /// <summary>
        /// Build a prompt that instructs the model to return a strict JSON object:
        /// {
        ///   "summary": "...",
        ///   "ats_score": 0-100,
        ///   "recommended_improvements": ["...","..."]
        /// }
        /// Ensure the model output is ONLY the JSON object and nothing else.
        /// </summary>
        public static string BuildResumeAnalysisPrompt(string resumeText)
        {
            // keep prompt concise; could be extended with system instructions if using Chat API
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("You are an expert résumé reviewer and ATS analyzer.");
            sb.AppendLine("Given the resume text below, produce a single valid JSON object ONLY (no commentary).");
            sb.AppendLine("The JSON object MUST include these keys:");
            sb.AppendLine(" - summary: a 2-3 sentence summary of the candidate's profile.");
            sb.AppendLine(" - ats_score: an integer 0-100 estimating ATS compatibility.");
            sb.AppendLine(" - recommended_improvements: an array of short bullet suggestions (strings).");
            sb.AppendLine("Return strictly JSON (no backticks, no markdown).");
            sb.AppendLine();
            sb.AppendLine("Resume text:");
            sb.AppendLine(resumeText);
            sb.AppendLine();
            sb.AppendLine("Important: If you cannot extract anything, return an object with empty summary, ats_score 0, and empty array for recommended_improvements.");
            return sb.ToString();
        }

        internal static object BuildInterviewQuestionsPrompt(string resumeText, string? jobDescription, int count)
        {
            throw new NotImplementedException();
        }
    }
}
