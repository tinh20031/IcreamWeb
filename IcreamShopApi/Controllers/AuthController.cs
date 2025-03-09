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

        public AuthController(CreamDbContext context, IConfiguration configuration, IUserService userService)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
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
                PhoneNumber = registerModel.Phone,
                Address = registerModel.Address,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerModel.Password),
                Role = registerModel.Role // "Customer" or "Admin"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        // Đăng nhập và trả về token JWT
        /*[HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginModel.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginModel.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }*/
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginModel.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginModel.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            var token = _userService.GenerateJwtToken(user);  // Gọi từ UserService

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
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Role { get; set; } = "Customer";
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
