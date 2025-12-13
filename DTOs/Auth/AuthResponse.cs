namespace HireZ.DTOs.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public string Email { get; set; } = null!;
    }
}
