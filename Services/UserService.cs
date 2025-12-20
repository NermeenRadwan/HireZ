using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HireZ.Data;
using HireZ.DTOs.Auth;
using HireZ.Models;
using HireZ.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HireZ.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public UserService(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return (false, "Email and password are required.");

            var exists = await _db.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower());
            if (exists) return (false, "Email already registered.");

            var user = new User
            {
                Email = request.Email,
                PasswordHash = PasswordHasher.Hash(request.Password),
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, AuthResponse? Response, string? Error)> AuthenticateAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return (false, null, "Email and password are required.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
            if (user == null) return (false, null, "Invalid credentials.");

            var valid = PasswordHasher.Verify(request.Password, user.PasswordHash);
            if (!valid) return (false, null, "Invalid credentials.");

            // generate JWT
            var token = GenerateJwtToken(user);
            return (true, token, null);
        }

        private AuthResponse GenerateJwtToken(User user)
        {
            // Get jwt settings
            var jwtSettings = _config.GetSection("Jwt");
            var key = jwtSettings.GetValue<string>("Key");

            if (string.IsNullOrWhiteSpace(key))
            {
                // Fail fast — Program.cs already enforces this, but guard here for safety
                throw new InvalidOperationException("JWT key is not configured. Set Jwt:Key in configuration.");
            }

            var issuer = jwtSettings.GetValue<string>("Issuer") ?? "HireZApi";
            var audience = jwtSettings.GetValue<string>("Audience") ?? "HireZClients";
            var expiryMinutes = jwtSettings.GetValue<int?>("ExpiryMinutes") ?? 1440;

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(key);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("userId", user.Id.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return new AuthResponse
            {
                Token = token,
                ExpiresAt = tokenDescriptor.Expires!.Value,
                Email = user.Email
            };
        }
    }
}
