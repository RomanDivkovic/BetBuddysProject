using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyDotNetProject.Models;

namespace MyDotNetProject.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthService(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
                return null;

            if (!await VerifyPasswordAsync(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<User> RegisterAsync(string email, string password, string? displayName)
        {
            if (await UserExistsAsync(email))
                throw new InvalidOperationException("User already exists");

            var passwordHash = await HashPasswordAsync(password);
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                DisplayName = displayName ?? email.Split('@')[0],
                PasswordHash = passwordHash,
                EmailVerificationToken = await GenerateEmailVerificationTokenAsync(null)
            };

            return await _userService.CreateUserAsync(user);
        }

        public Task<string> GenerateJwtTokenAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "your-secret-key-here-make-it-long-and-secure");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.DisplayName ?? user.Email)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"] ?? "your-app",
                Audience = _configuration["Jwt:Audience"] ?? "your-app-users"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(tokenHandler.WriteToken(token));
        }

        public Task<bool> VerifyPasswordAsync(string password, string passwordHash)
        {
            return Task.FromResult(BCrypt.Net.BCrypt.Verify(password, passwordHash));
        }

        public Task<string> HashPasswordAsync(string password)
        {
            return Task.FromResult(BCrypt.Net.BCrypt.HashPassword(password));
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userService.GetUserByEmailAsync(email);
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            return user != null;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            var token = Guid.NewGuid().ToString();
            user.ResetToken = token;
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);
            await _userService.UpdateUserAsync(user);
            return token;
        }

        public async Task<bool> VerifyPasswordResetTokenAsync(string token)
        {
            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.ResetToken == token && u.ResetTokenExpires > DateTime.UtcNow);
            return user != null;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.ResetToken == token && u.ResetTokenExpires > DateTime.UtcNow);
            if (user == null)
                return false;

            user.PasswordHash = await HashPasswordAsync(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpires = null;
            await _userService.UpdateUserAsync(user);
            return true;
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.EmailVerificationToken == token);
            if (user == null)
                return false;

            user.EmailVerified = true;
            user.EmailVerificationToken = null;
            await _userService.UpdateUserAsync(user);
            return true;
        }

        public Task<string> GenerateEmailVerificationTokenAsync(User? user)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }
    }
}
