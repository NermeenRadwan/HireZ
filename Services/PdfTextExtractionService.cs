using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using UglyToad.PdfPig;

namespace HireZ.Services
{
    public class PdfTextExtractionService : ITextExtractionService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<PdfTextExtractionService> _logger;

        public PdfTextExtractionService(IWebHostEnvironment env, ILogger<PdfTextExtractionService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public Task<string> ExtractTextAsync(string relativeFilePath, CancellationToken cancellation = default)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, relativeFilePath);
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("PDF not found at path: {path}", fullPath);
                return Task.FromResult(string.Empty);
            }

            var sb = new StringBuilder();

            try
            {
                // PdfPig's API: PdfDocument.Open(path)
                using (var document = PdfDocument.Open(fullPath))
                {
                    foreach (var page in document.GetPages())
                    {
                        if (cancellation.IsCancellationRequested) break;
                        // page.Text returns the page's plain text
                        sb.AppendLine(page.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract text from PDF {path}", fullPath);
            }

            return Task.FromResult(sb.ToString());
        }
    }
}
