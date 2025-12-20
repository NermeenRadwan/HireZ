using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace HireZ.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileStorageService> _logger;
        private readonly string _uploadsFolderName = "uploads";

        public FileStorageService(IWebHostEnvironment env, ILogger<FileStorageService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, CancellationToken cancellation = default)
        {
            var uploadsRoot = Path.Combine(_env.ContentRootPath, _uploadsFolderName);
            if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);

            // create unique filename
            var unique = $"{Guid.NewGuid():N}_{Path.GetFileName(fileName)}";
            var filePath = Path.Combine(uploadsRoot, unique);

            // Save file
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await fileStream.CopyToAsync(fs, 81920, cancellation);
            }

            // Return relative path (uploads/...)
            return Path.Combine(_uploadsFolderName, unique).Replace('\\', '/');
        }

        public Task DeleteFileAsync(string relativePath, CancellationToken cancellation = default)
        {
            try
            {
                var full = Path.Combine(_env.ContentRootPath, relativePath);
                if (File.Exists(full)) File.Delete(full);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete file {path}", relativePath);
            }
            return Task.CompletedTask;
        }
    }
}
