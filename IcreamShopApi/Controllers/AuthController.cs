using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IcreamShopApi.Models;
using Microsoft.EntityFrameworkCore;
using IcreamShopApi.Data;
using Microsoft.AspNetCore.Authorization;

namespace IcreamShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CreamDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(CreamDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
		[HttpPost("login")]
		public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginModel.Email);
			if (user == null || !BCrypt.Net.BCrypt.Verify(loginModel.Password, user.PasswordHash))
				return Unauthorized("Invalid email or password.");

			var claims = new[]
			{
		new Claim(ClaimTypes.Name, user.Email),
		new Claim(ClaimTypes.Role, user.Role),
		new Claim("userId", user.UserId.ToString()) // Thêm userId vào claims
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
				token = new JwtSecurityTokenHandler().WriteToken(token),
				userId = user.UserId, // Thêm userId vào phản hồi
				email = user.Email,
				role = user.Role
			});
		}


		[HttpGet("me")]
		[Authorize] // Yêu cầu token JWT
		public async Task<ActionResult<UserDTO>> GetUserInfo()
		{
			var email = User.FindFirst(ClaimTypes.Name)?.Value;
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
			if (user == null)
				return NotFound("User not found");

			return Ok(new UserDTO
			{
				UserId = user.UserId,
				Email = user.Email,
				Role = user.Role
			});
		}

		public class UserDTO
		{
			public int UserId { get; set; }
			public string Email { get; set; }
			public string Role { get; set; }
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
