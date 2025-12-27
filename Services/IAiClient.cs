using System.Threading.Tasks;

namespace HireZ.Services
{
    public interface IAiClient
    {
        Task<string?> GenerateAsync(string prompt);
        Task<string?> GenerateAsync(object prompt);
    }
}
