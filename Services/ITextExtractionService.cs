namespace HireZ.Services
{
    public interface ITextExtractionService
    {
        Task<string> ExtractTextAsync(string relativeFilePath, CancellationToken cancellation = default);
    }
}
