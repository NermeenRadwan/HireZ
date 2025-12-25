using System.Threading.Tasks;

namespace HireZ.Services
{
    /// <summary>
    /// Simple local AI client abstraction used by InterviewService.
    /// An adapter implementation will attempt to call your existing IAiService if present.
    /// </summary>
    public interface IAiClient
    {
        /// <summary>
        /// Generate a text response from a prompt. Returns null when AI is unavailable or failed.
        /// </summary>
        Task<string?> GenerateAsync(string prompt);
    }
}
