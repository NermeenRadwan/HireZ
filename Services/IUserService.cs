using HireZ.DTOs.Auth;

namespace HireZ.Services
{
    public interface IUserService
    {
        Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request);
        Task<(bool Success, AuthResponse? Response, string? Error)> AuthenticateAsync(LoginRequest request);
    }
}
