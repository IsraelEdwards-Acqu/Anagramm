using Anagram.Server.Data;
using Anagram.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Anagram.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(
            [FromBody] UserDto userDto,
            [FromServices] AnagramDbContext db)
        {
            if (string.IsNullOrWhiteSpace(userDto.Username))
                return BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(userDto.Email))
                return BadRequest("Email is required.");

            if (string.IsNullOrWhiteSpace(userDto.Password))
                return BadRequest("Password is required.");

            var usernameExists = await db.Users.AnyAsync(u =>
                u.Username.ToLower() == userDto.Username.ToLower());

            if (usernameExists)
                return BadRequest("Username already exists.");

            var emailExists = await db.Users.AnyAsync(u =>
                u.Email.ToLower() == userDto.Email.ToLower());

            if (emailExists)
                return BadRequest("Email already exists.");

            var hashedPassword =
                BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var user = new User
            {
                Username = userDto.Username.Trim(),
                Name = string.IsNullOrWhiteSpace(userDto.Name)
                    ? userDto.Username.Trim()
                    : userDto.Name.Trim(),

                Email = userDto.Email.Trim(),
                PasswordHash = hashedPassword,

                AvatarUrl = "/images/anagramUser.png",

                Status = "Online",

                LastSeen = DateTime.UtcNow,

                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(user);

            await db.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Id,
                    user.Username,
                    user.Name,
                    user.Email,
                    user.AvatarUrl
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] UserDto userDto,
            [FromServices] AnagramDbContext db)
        {
            if (string.IsNullOrWhiteSpace(userDto.Email))
                return BadRequest("Email is required.");

            if (string.IsNullOrWhiteSpace(userDto.Password))
                return BadRequest("Password is required.");

            var user = await db.Users
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == userDto.Email.ToLower());

            if (user == null)
                return Unauthorized("Invalid email or password.");

            var passwordValid =
                BCrypt.Net.BCrypt.Verify(
                    userDto.Password,
                    user.PasswordHash);

            if (!passwordValid)
                return Unauthorized("Invalid email or password.");

            user.LastSeen = DateTime.UtcNow;

            await db.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Id,
                    user.Username,
                    user.Name,
                    user.Email,
                    user.AvatarUrl
                }
            });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                UserId =
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value,

                Username =
                    User.FindFirst(ClaimTypes.Name)?.Value,

                Email =
                    User.FindFirst(ClaimTypes.Email)?.Value,

                DisplayName =
                    User.FindFirst("displayName")?.Value
            });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                new Claim(
                    ClaimTypes.Email,
                    user.Email),

                new Claim(
                    "displayName",
                    string.IsNullOrWhiteSpace(user.Name)
                        ? user.Username
                        : user.Name)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _config["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }

    public class UserDto
    {
        public string Username { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}