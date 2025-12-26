namespace HireZ.Services
{
    public interface IFileStorageService
    {
        /// <summary>Save the uploaded stream to a storage path and return the saved filepath relative to app root.</summary>
        Task<string> SaveFileAsync(Stream fileStream, string fileName, CancellationToken cancellation = default);

        /// <summary>Delete a stored file (optional).</summary>
        Task DeleteFileAsync(string relativePath, CancellationToken cancellation = default);
    }
}
