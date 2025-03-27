using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IcreamShopApi.Models;
using Microsoft.EntityFrameworkCore;
using IcreamShopApi.Data;
using IcreamShopApi.DTOs;
using IcreamShopApi.Services;

namespace IcreamShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CreamDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public AuthController(CreamDbContext context, IConfiguration configuration, IUserService userService, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
            _emailService = emailService;
        }

        // Đăng ký người dùng mới

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterModel registerModel)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerModel.Email);
            if (existingUser != null)
                return BadRequest("Email already in use.");

            var user = new User
            {
                FullName = registerModel.FullName,
                Email = registerModel.Email,
                PhoneNumber = registerModel.PhoneNumber,
                Address = registerModel.Address,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerModel.Password),
                Role = registerModel.Role,
                IsEmailVerified = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Tạo token xác nhận
            var verificationToken = Guid.NewGuid().ToString();
            var tokenEntity = new VerificationToken
            {
                UserId = user.UserId,
                Token = verificationToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
            _context.VerificationTokens.Add(tokenEntity);
            await _context.SaveChangesAsync();

            // Gửi email xác nhận
            var baseUrl = _configuration["Application:BaseUrl"];
            var verificationLink = $"{baseUrl}/api/auth/verify?token={verificationToken}";
            var emailBody = $"Welcome to Ice Cream Shop!\n\nPlease verify your email by clicking this link: {verificationLink}\n\nThis link expires in 24 hours.";
            await _emailService.SendEmailAsync(user.Email, "Verify Your Account", emailBody);

            return Ok("User registered successfully. Please check your email to verify your account.");
        }

        [HttpGet("verify")]
        public async Task<ActionResult> VerifyEmail(string token)
        {
            var verificationToken = await _context.VerificationTokens
                .FirstOrDefaultAsync(vt => vt.Token == token && vt.ExpiresAt > DateTime.UtcNow);

            if (verificationToken == null)
                return BadRequest("Invalid or expired verification token.");

            var user = await _context.Users.FindAsync(verificationToken.UserId);
            if (user == null)
                return NotFound("User not found.");

            user.IsEmailVerified = true;
            _context.VerificationTokens.Remove(verificationToken);
            await _context.SaveChangesAsync();

            return Ok("Email verified successfully. You can now log in.");
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginModel.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginModel.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            if (!user.IsEmailVerified)
                return Unauthorized("Please verify your email before logging in.");

            var token = _userService.GenerateJwtToken(user);

            var authResponse = new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddDays(1),
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };

            return Ok(authResponse);
        }

    }

    // Model cho Register và Login
    public class RegisterModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Role { get; set; } = "user";
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
