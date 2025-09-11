using Microsoft.AspNetCore.Mvc;
using MyDotNetProject.Models;
using MyDotNetProject.Services;
using System.Threading.Tasks;

namespace MyDotNetProject.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request.Email, request.Password, request.DisplayName);
                var token = await _authService.GenerateJwtTokenAsync(user);

                return Ok(new AuthResponse
                {
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        DisplayName = user.DisplayName,
                        EmailVerified = user.EmailVerified
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _authService.AuthenticateAsync(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = await _authService.GenerateJwtTokenAsync(user);

            return Ok(new AuthResponse
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    EmailVerified = user.EmailVerified
                }
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _authService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal if email exists or not for security
                return Ok(new { message = "If the email exists, a password reset link has been sent" });
            }

            var resetToken = await _authService.GeneratePasswordResetTokenAsync(user);
            // TODO: Send email with reset token
            // For now, just return the token (in production, send via email)

            return Ok(new
            {
                message = "Password reset token generated",
                resetToken = resetToken // Remove this in production
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
            if (!result)
            {
                return BadRequest(new { message = "Invalid or expired reset token" });
            }

            return Ok(new { message = "Password reset successfully" });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var result = await _authService.VerifyEmailAsync(request.Token);
            if (!result)
            {
                return BadRequest(new { message = "Invalid verification token" });
            }

            return Ok(new { message = "Email verified successfully" });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            // For now, just return success
            // In a full implementation, you'd validate the current token and issue a new one
            return Ok(new { message = "Token refresh not implemented yet" });
        }
    }

    // Request/Response models
    public class RegisterRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? DisplayName { get; set; }
    }

    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public required string Email { get; set; }
    }

    public class ResetPasswordRequest
    {
        public required string Token { get; set; }
        public required string NewPassword { get; set; }
    }

    public class VerifyEmailRequest
    {
        public required string Token { get; set; }
    }

    public class AuthResponse
    {
        public required string Token { get; set; }
        public required UserDto User { get; set; }
    }

    public class UserDto
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public string? DisplayName { get; set; }
        public bool EmailVerified { get; set; }
    }
}
